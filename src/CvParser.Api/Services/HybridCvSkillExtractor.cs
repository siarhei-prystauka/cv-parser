using System.Text.Json;
using System.Text.RegularExpressions;

namespace CvParser.Api.Services;

/// <summary>
/// Hybrid CV skill extractor that uses taxonomy-first matching with LLM fallback.
/// </summary>
public class HybridCvSkillExtractor : ICvSkillExtractor
{
    private readonly CvTextExtractorFactory _textExtractorFactory;
    private readonly ILlmSkillExtractor _llmExtractor;
    private readonly ILogger<HybridCvSkillExtractor> _logger;
    private readonly SkillsTaxonomy _taxonomy;

    public HybridCvSkillExtractor(
        CvTextExtractorFactory textExtractorFactory,
        ILlmSkillExtractor llmExtractor,
        IWebHostEnvironment environment,
        ILogger<HybridCvSkillExtractor> logger)
    {
        _textExtractorFactory = textExtractorFactory;
        _llmExtractor = llmExtractor;
        _logger = logger;
        _taxonomy = LoadTaxonomy(environment);
    }

    /// <summary>
    /// Extracts skills from a CV file using hybrid taxonomy-first approach.
    /// </summary>
    public async Task<IReadOnlyList<string>> ExtractSkillsAsync(
        Stream cvFileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting skill extraction for file: {FileName}, ContentType: {ContentType}", fileName, contentType);

        try
        {
            // Step 1: Extract text from CV
            var extractor = _textExtractorFactory.GetExtractor(contentType);
            var cvText = await extractor.ExtractTextAsync(cvFileStream, contentType);

            if (string.IsNullOrWhiteSpace(cvText))
            {
                _logger.LogWarning("Extracted text is empty for file: {FileName}", fileName);
                return Array.Empty<string>();
            }

            _logger.LogInformation("Extracted {Length} characters from CV", cvText.Length);

            // Step 2: Normalize and tokenize text
            var normalizedText = NormalizeText(cvText);
            
            // Step 3: Match against taxonomy
            var taxonomyMatches = MatchTaxonomy(normalizedText);
            _logger.LogInformation("Taxonomy matched {Count} skills", taxonomyMatches.Count());

            // Step 4: Use LLM for additional skill extraction
            var llmSkills = await _llmExtractor.ExtractSkillsAsync(cvText, cancellationToken);
            _logger.LogInformation("LLM extracted {Count} skills", llmSkills.Count());

            // Step 5: Merge and deduplicate results
            var allSkills = taxonomyMatches.Concat(llmSkills)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            _logger.LogInformation("Total unique skills extracted: {Count}", allSkills.Count);
            return allSkills;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting skills from CV");
            throw;
        }
    }

    /// <summary>
    /// Normalizes text for better matching (lowercase, remove special chars).
    /// </summary>
    private static string NormalizeText(string text)
    {
        // Convert to lowercase and normalize whitespace
        return Regex.Replace(text.ToLowerInvariant(), @"\s+", " ");
    }

    /// <summary>
    /// Matches skills from the taxonomy in the CV text.
    /// </summary>
    private IEnumerable<string> MatchTaxonomy(string normalizedText)
    {
        var matches = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var skill in _taxonomy.Skills)
        {
            // Check main skill name
            if (ContainsSkillPattern(normalizedText, skill.Name))
            {
                matches.Add(skill.Name);
                continue;
            }

            // Check aliases
            foreach (var alias in skill.Aliases)
            {
                if (ContainsSkillPattern(normalizedText, alias))
                {
                    matches.Add(skill.Name); // Add canonical name, not alias
                    break;
                }
            }
        }

        return matches;
    }

    /// <summary>
    /// Checks if text contains a skill pattern with word boundaries.
    /// </summary>
    private static bool ContainsSkillPattern(string text, string skill)
    {
        var normalizedSkill = skill.ToLowerInvariant();
        
        // Use word boundary matching to avoid partial matches
        // Special handling for skills with dots or special characters
        var pattern = Regex.Escape(normalizedSkill);
        
        // Replace escaped dots with actual dot patterns
        pattern = pattern.Replace(@"\.", @"\.?");
        
        // Add word boundaries, but handle special cases
        pattern = $@"(?<![a-z0-9]){pattern}(?![a-z0-9])";

        return Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Loads the skills taxonomy from JSON file.
    /// </summary>
    private static SkillsTaxonomy LoadTaxonomy(IWebHostEnvironment environment)
    {
        var taxonomyPath = Path.Combine(environment.ContentRootPath, "Data", "skills-taxonomy.json");
        
        if (!File.Exists(taxonomyPath))
        {
            throw new FileNotFoundException($"Skills taxonomy file not found at: {taxonomyPath}");
        }

        var json = File.ReadAllText(taxonomyPath);
        var taxonomy = JsonSerializer.Deserialize<SkillsTaxonomy>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (taxonomy is null || taxonomy.Skills is null || taxonomy.Skills.Length == 0)
        {
            throw new InvalidOperationException("Failed to load skills taxonomy or taxonomy is empty");
        }

        return taxonomy;
    }

    /// <summary>
    /// Skills taxonomy model.
    /// </summary>
    private record SkillsTaxonomy
    {
        public required Skill[] Skills { get; init; }
    }

    /// <summary>
    /// Individual skill model.
    /// </summary>
    private record Skill
    {
        public required string Name { get; init; }
        public required string Category { get; init; }
        public required string[] Aliases { get; init; }
    }
}

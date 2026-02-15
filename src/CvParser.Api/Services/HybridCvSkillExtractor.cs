using System.Text.Json;
using System.Text.RegularExpressions;
using CvParser.Api.Repositories;

namespace CvParser.Api.Services;

/// <summary>
/// Taxonomy-first skill extractor with LLM fallback.
/// </summary>
public class HybridCvSkillExtractor : ICvSkillExtractor
{
    private readonly ICvTextExtractorFactory _textExtractorFactory;
    private readonly ILlmSkillExtractor _llmExtractor;
    private readonly ILogger<HybridCvSkillExtractor> _logger;
    private readonly ISettingsRepository _settingsRepository;
    private readonly SkillsTaxonomy _taxonomy;

    public HybridCvSkillExtractor(
        ICvTextExtractorFactory textExtractorFactory,
        ILlmSkillExtractor llmExtractor,
        IWebHostEnvironment environment,
        ISettingsRepository settingsRepository,
        ILogger<HybridCvSkillExtractor> logger)
    {
        _textExtractorFactory = textExtractorFactory;
        _llmExtractor = llmExtractor;
        _logger = logger;
        _settingsRepository = settingsRepository;
        _taxonomy = LoadTaxonomy(environment);
    }

    public async Task<IReadOnlyList<string>> ExtractSkillsAsync(
        Stream cvFileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting skill extraction for file: {FileName}, ContentType: {ContentType}", fileName, contentType);

        try
        {
            var extractor = _textExtractorFactory.GetExtractor(contentType);
            var cvText = await extractor.ExtractTextAsync(cvFileStream, contentType);

            if (string.IsNullOrWhiteSpace(cvText))
            {
                _logger.LogWarning("Extracted text is empty for file: {FileName}", fileName);
                return Array.Empty<string>();
            }

            _logger.LogInformation("Extracted {Length} characters from CV", cvText.Length);

            var normalizedText = NormalizeText(cvText);

            var taxonomyMatches = MatchTaxonomy(normalizedText);
            _logger.LogInformation("Taxonomy matched {Count} skills", taxonomyMatches.Count());

            var options = await _settingsRepository.GetSkillExtractionOptionsAsync();
            
            IEnumerable<string> llmSkills;
            if (options.LlmFallbackOnly && taxonomyMatches.Any())
            {
                _logger.LogInformation("LlmFallbackOnly mode enabled and taxonomy found skills - skipping LLM call");
                llmSkills = Array.Empty<string>();
            }
            else
            {
                llmSkills = await _llmExtractor.ExtractSkillsAsync(cvText, cancellationToken);
                _logger.LogInformation("LLM extracted {Count} skills", llmSkills.Count());
            }

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

    private static string NormalizeText(string text)
    {
        return Regex.Replace(text.ToLowerInvariant(), @"\s+", " ");
    }

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

    private static bool ContainsSkillPattern(string text, string skill)
    {
        var normalizedSkill = skill.ToLowerInvariant();

        // Word boundary matching to avoid partial matches
        var pattern = Regex.Escape(normalizedSkill);
        pattern = pattern.Replace(@"\.", @"\.?");
        pattern = $@"(?<![a-z0-9]){pattern}(?![a-z0-9])";

        return Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase);
    }

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

    private record SkillsTaxonomy
    {
        public required Skill[] Skills { get; init; }
    }

    private record Skill
    {
        public required string Name { get; init; }
        public required string Category { get; init; }
        public required string[] Aliases { get; init; }
    }
}

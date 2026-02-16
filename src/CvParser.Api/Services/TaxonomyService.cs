using System.Text.Json;
using CvParser.Api.Models.Responses;

namespace CvParser.Api.Services;

public sealed class TaxonomyService : ITaxonomyService
{
    private readonly IReadOnlyList<TaxonomySkill> _taxonomy;

    public TaxonomyService(IWebHostEnvironment environment)
    {
        _taxonomy = LoadTaxonomy(environment);
    }

    public IReadOnlyList<TaxonomySkill> GetTaxonomy() => _taxonomy;

    private static IReadOnlyList<TaxonomySkill> LoadTaxonomy(IWebHostEnvironment environment)
    {
        var taxonomyPath = Path.Combine(environment.ContentRootPath, "Data", "skills-taxonomy.json");

        if (!File.Exists(taxonomyPath))
        {
            throw new FileNotFoundException($"Skills taxonomy file not found at: {taxonomyPath}");
        }

        var json = File.ReadAllText(taxonomyPath);
        var taxonomy = JsonSerializer.Deserialize<TaxonomyJson>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (taxonomy is null || taxonomy.Skills is null)
        {
            throw new InvalidOperationException("Failed to deserialize skills taxonomy");
        }

        var skills = taxonomy.Skills
            .Select(s => new TaxonomySkill(s.Name, s.Category, s.Aliases))
            .ToList();

        return skills.AsReadOnly();
    }

    private sealed record TaxonomyJson
    {
        public required SkillJson[] Skills { get; init; }
    }

    private sealed record SkillJson
    {
        public required string Name { get; init; }
        public required string Category { get; init; }
        public required string[] Aliases { get; init; }
    }
}

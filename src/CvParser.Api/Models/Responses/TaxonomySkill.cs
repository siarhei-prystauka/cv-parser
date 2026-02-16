namespace CvParser.Api.Models.Responses;

public sealed record TaxonomySkill(
    string Name,
    string Category,
    IReadOnlyList<string> Aliases
);

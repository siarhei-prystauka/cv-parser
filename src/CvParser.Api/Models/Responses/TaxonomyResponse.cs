namespace CvParser.Api.Models.Responses;

/// <summary>
/// Skills taxonomy read-only view.
/// </summary>
public sealed record TaxonomyResponse(
    IReadOnlyList<TaxonomySkill> Skills
);

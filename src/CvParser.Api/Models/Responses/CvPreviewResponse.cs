namespace CvParser.Api.Models.Responses;

/// <summary>
/// Extracted skills returned from a CV preview.
/// </summary>
public sealed record CvPreviewResponse(
    string FileName,
    IReadOnlyList<string> ExtractedSkills
);

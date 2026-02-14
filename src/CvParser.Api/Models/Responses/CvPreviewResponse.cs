namespace CvParser.Api.Models.Responses;

/// <summary>
/// Returns the extracted skills from a CV preview.
/// </summary>
public sealed record CvPreviewResponse(
    string FileName,
    IReadOnlyList<string> ExtractedSkills
);

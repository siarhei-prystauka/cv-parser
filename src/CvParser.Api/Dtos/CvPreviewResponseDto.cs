namespace CvParser.Api.Dtos;

/// <summary>
/// Returns the extracted skills from a CV preview.
/// </summary>
public sealed record CvPreviewResponseDto(
    string FileName,
    IReadOnlyList<string> ExtractedSkills
);

namespace CvParser.Api.Services;

/// <summary>
/// Extracts skills from CV files.
/// </summary>
public interface ICvSkillExtractor
{
    /// <summary>
    /// Extracts skills from a CV file for preview.
    /// </summary>
    Task<IReadOnlyList<string>> ExtractSkillsAsync(Stream fileStream, string fileName, CancellationToken cancellationToken);
}

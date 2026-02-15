namespace CvParser.Api.Services;

/// <summary>
/// Extracts skills from CV files.
/// </summary>
public interface ICvSkillExtractor
{
    /// <summary>
    /// Extracts skills from a CV file for preview.
    /// </summary>
    /// <param name="fileStream">The CV file stream.</param>
    /// <param name="fileName">The name of the CV file.</param>
    /// <param name="contentType">The MIME content type of the file.</param>
    /// <param name="cancellationToken">Token used to observe cancellation requests for long-running operations.</param>
    /// <returns>A list of extracted skill names.</returns>
    Task<IReadOnlyList<string>> ExtractSkillsAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);
}

namespace CvParser.Api.Services;

/// <summary>
/// Contract for extracting plain text from CV files.
/// </summary>
public interface ICvTextExtractor
{
    /// <summary>
    /// Extracts plain text from a CV file stream.
    /// </summary>
    /// <param name="fileStream">The CV file stream.</param>
    /// <param name="contentType">The MIME content type of the file.</param>
    /// <returns>The extracted plain text.</returns>
    /// <exception cref="NotSupportedException">Thrown when the content type is not supported.</exception>
    /// <exception cref="InvalidOperationException">Thrown when text extraction fails.</exception>
    Task<string> ExtractTextAsync(Stream fileStream, string contentType);
}

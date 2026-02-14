namespace CvParser.Api.Services;

/// <summary>
/// Factory for creating appropriate CV text extractors based on content type.
/// </summary>
public class CvTextExtractorFactory
{
    private readonly PdfTextExtractor _pdfExtractor;
    private readonly ILogger<CvTextExtractorFactory> _logger;

    public CvTextExtractorFactory(
        PdfTextExtractor pdfExtractor,
        ILogger<CvTextExtractorFactory> logger)
    {
        _pdfExtractor = pdfExtractor;
        _logger = logger;
    }

    /// <summary>
    /// Gets the appropriate text extractor for the given content type.
    /// </summary>
    /// <param name="contentType">The MIME content type.</param>
    /// <returns>The text extractor instance.</returns>
    /// <exception cref="NotSupportedException">Thrown when the content type is not supported.</exception>
    public ICvTextExtractor GetExtractor(string contentType)
    {
        _logger.LogDebug("Getting extractor for content type: {ContentType}", contentType);

        return contentType switch
        {
            "application/pdf" => _pdfExtractor,
            // TODO: Add DOCX support (see GitHub issue for DOCX extraction)
            // "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => _docxExtractor,
            _ => throw new NotSupportedException($"Content type '{contentType}' is not supported. Currently only PDF files are supported.")
        };
    }

    /// <summary>
    /// Checks if the content type is supported.
    /// </summary>
    public static bool IsSupported(string contentType)
    {
        return contentType is "application/pdf";
        // TODO: Add DOCX when support is implemented
        // or "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
    }
}

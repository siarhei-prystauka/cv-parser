namespace CvParser.Api.Services;

/// <summary>
/// Selects the appropriate text extractor by matching content type.
/// </summary>
public class CvTextExtractorFactory : ICvTextExtractorFactory
{
    private readonly IEnumerable<ICvTextExtractor> _extractors;
    private readonly ILogger<CvTextExtractorFactory> _logger;

    public CvTextExtractorFactory(
        IEnumerable<ICvTextExtractor> extractors,
        ILogger<CvTextExtractorFactory> logger)
    {
        _extractors = extractors;
        _logger = logger;
    }

    public ICvTextExtractor GetExtractor(string contentType)
    {
        _logger.LogDebug("Getting extractor for content type: {ContentType}", contentType);

        var extractor = _extractors
            .FirstOrDefault(e => string.Equals(e.SupportedContentType, contentType, StringComparison.OrdinalIgnoreCase));

        if (extractor is null)
        {
            throw new NotSupportedException(
                $"Content type '{contentType}' is not supported. Supported types: {string.Join(", ", _extractors.Select(e => e.SupportedContentType))}.");
        }

        return extractor;
    }

    public bool IsSupported(string contentType)
    {
        return _extractors.Any(e => string.Equals(e.SupportedContentType, contentType, StringComparison.OrdinalIgnoreCase));
    }
}

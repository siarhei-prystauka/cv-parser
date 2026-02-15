namespace CvParser.Api.Services;

public interface ICvTextExtractor
{
    string SupportedContentType { get; }
    Task<string> ExtractTextAsync(Stream fileStream, string contentType);
}

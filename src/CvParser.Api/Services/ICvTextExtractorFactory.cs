namespace CvParser.Api.Services;

public interface ICvTextExtractorFactory
{
    ICvTextExtractor GetExtractor(string contentType);
    bool IsSupported(string contentType);
}

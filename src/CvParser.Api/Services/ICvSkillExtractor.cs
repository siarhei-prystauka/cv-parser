namespace CvParser.Api.Services;

public interface ICvSkillExtractor
{
    Task<IReadOnlyList<string>> ExtractSkillsAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);
}

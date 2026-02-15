namespace CvParser.Api.Services;

public interface ILlmSkillExtractor
{
    Task<IEnumerable<string>> ExtractSkillsAsync(string cvText, CancellationToken cancellationToken = default);
}

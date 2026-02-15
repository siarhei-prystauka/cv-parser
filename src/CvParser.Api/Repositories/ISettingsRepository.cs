using CvParser.Api.Models.Options;

namespace CvParser.Api.Repositories;

public interface ISettingsRepository
{
    Task<SkillExtractionOptions> GetSkillExtractionOptionsAsync();
    Task UpdateSkillExtractionOptionsAsync(SkillExtractionOptions options);
    Task<GroqOptions> GetGroqOptionsAsync();
    Task UpdateGroqOptionsAsync(GroqOptions options);
}

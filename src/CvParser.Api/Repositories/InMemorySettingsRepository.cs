using CvParser.Api.Models;
using CvParser.Api.Models.Options;
using Microsoft.Extensions.Options;

namespace CvParser.Api.Repositories;

/// <summary>
/// In-memory settings store for runtime configuration.
/// </summary>
public sealed class InMemorySettingsRepository : ISettingsRepository
{
    private ApplicationSetting _setting;
    private readonly object _lock = new();

    public InMemorySettingsRepository(IOptions<SkillExtractionOptions> skillExtractionOptions, IOptions<GroqOptions> groqOptions)
    {
        _setting = new ApplicationSetting
        {
            LlmFallbackOnly = skillExtractionOptions.Value.LlmFallbackOnly,
            LlmModel = groqOptions.Value.Model ?? "llama-3.3-70b-versatile"
        };
    }

    public Task<ApplicationSetting> GetAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(new ApplicationSetting
            {
                Id = _setting.Id,
                LlmFallbackOnly = _setting.LlmFallbackOnly,
                LlmModel = _setting.LlmModel,
                UpdatedAt = _setting.UpdatedAt
            });
        }
    }

    public Task UpdateAsync(ApplicationSetting setting)
    {
        lock (_lock)
        {
            _setting.LlmFallbackOnly = setting.LlmFallbackOnly;
            _setting.LlmModel = setting.LlmModel;
            _setting.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }
}

using CvParser.Api.Models.Options;
using Microsoft.Extensions.Options;

namespace CvParser.Api.Repositories;

/// <summary>
/// In-memory settings store for runtime configuration.
/// </summary>
public sealed class InMemorySettingsRepository : ISettingsRepository
{
    private SkillExtractionOptions _skillExtractionOptions;
    private GroqOptions _groqOptions;
    private readonly object _lock = new();

    public InMemorySettingsRepository(IOptions<SkillExtractionOptions> skillExtractionOptions, IOptions<GroqOptions> groqOptions)
    {
        _skillExtractionOptions = skillExtractionOptions.Value;
        _groqOptions = groqOptions.Value;
    }

    public Task<SkillExtractionOptions> GetSkillExtractionOptionsAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(new SkillExtractionOptions { LlmFallbackOnly = _skillExtractionOptions.LlmFallbackOnly });
        }
    }

    public Task UpdateSkillExtractionOptionsAsync(SkillExtractionOptions options)
    {
        lock (_lock)
        {
            _skillExtractionOptions = options;
            return Task.CompletedTask;
        }
    }

    public Task<GroqOptions> GetGroqOptionsAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(new GroqOptions
            {
                ApiKey = _groqOptions.ApiKey,
                BaseUrl = _groqOptions.BaseUrl,
                Model = _groqOptions.Model,
                TimeoutSeconds = _groqOptions.TimeoutSeconds,
                MaxTokens = _groqOptions.MaxTokens
            });
        }
    }

    public Task UpdateGroqOptionsAsync(GroqOptions options)
    {
        lock (_lock)
        {
            _groqOptions = options;
            return Task.CompletedTask;
        }
    }
}

using CvParser.Api.Models.Options;

namespace CvParser.Api.Repositories;

/// <summary>
/// In-memory settings store for runtime configuration.
/// </summary>
public sealed class InMemorySettingsRepository : ISettingsRepository
{
    private SkillExtractionOptions _skillExtractionOptions;
    private GroqOptions _groqOptions;
    private readonly object _lock = new();

    public InMemorySettingsRepository(SkillExtractionOptions skillExtractionOptions, GroqOptions groqOptions)
    {
        _skillExtractionOptions = skillExtractionOptions;
        _groqOptions = groqOptions;
    }

    public Task<SkillExtractionOptions> GetSkillExtractionOptionsAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(_skillExtractionOptions);
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
            return Task.FromResult(_groqOptions);
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

using CvParser.Api.Data;
using CvParser.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CvParser.Api.Repositories;

/// <summary>
/// EF Core backed settings repository.
/// </summary>
public sealed class SqlSettingsRepository(ApplicationDbContext context) : ISettingsRepository
{
    private const string DefaultLlmModel = "llama-3.3-70b-versatile";

    public async Task<ApplicationSetting> GetAsync()
        => await context.ApplicationSettings.SingleOrDefaultAsync()
           ?? new ApplicationSetting { LlmFallbackOnly = false, LlmModel = DefaultLlmModel };

    public async Task UpdateAsync(ApplicationSetting setting)
    {
        var existing = await context.ApplicationSettings.SingleOrDefaultAsync();
        if (existing is null)
        {
            setting.UpdatedAt = DateTime.UtcNow;
            context.ApplicationSettings.Add(setting);
        }
        else
        {
            existing.LlmFallbackOnly = setting.LlmFallbackOnly;
            existing.LlmModel = setting.LlmModel;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
    }
}

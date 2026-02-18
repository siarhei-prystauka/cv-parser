using CvParser.Api.Data;
using CvParser.Api.Models;
using CvParser.Api.Models.Options;
using Microsoft.EntityFrameworkCore;

namespace CvParser.Api.Repositories;

/// <summary>
/// EF Core backed settings repository that persists configuration as key-value pairs.
/// </summary>
public sealed class SqlSettingsRepository(ApplicationDbContext context) : ISettingsRepository
{
    private const string SkillExtractionPrefix = "SkillExtraction:";
    private const string GroqPrefix = "Groq:";

    public async Task<SkillExtractionOptions> GetSkillExtractionOptionsAsync()
    {
        var settings = await GetSettingsByPrefixAsync(SkillExtractionPrefix);

        return new SkillExtractionOptions
        {
            LlmFallbackOnly = GetBool(settings, $"{SkillExtractionPrefix}LlmFallbackOnly", false)
        };
    }

    public async Task UpdateSkillExtractionOptionsAsync(SkillExtractionOptions options)
    {
        await UpsertAsync($"{SkillExtractionPrefix}LlmFallbackOnly", options.LlmFallbackOnly.ToString().ToLower());
    }

    public async Task<GroqOptions> GetGroqOptionsAsync()
    {
        var settings = await GetSettingsByPrefixAsync(GroqPrefix);

        return new GroqOptions
        {
            ApiKey = GetString(settings, $"{GroqPrefix}ApiKey", string.Empty),
            BaseUrl = GetString(settings, $"{GroqPrefix}BaseUrl", "https://api.groq.com/openai/v1/"),
            Model = GetString(settings, $"{GroqPrefix}Model", "llama-3.1-8b-instant"),
            TimeoutSeconds = GetInt(settings, $"{GroqPrefix}TimeoutSeconds", 30),
            MaxTokens = GetInt(settings, $"{GroqPrefix}MaxTokens", 1000)
        };
    }

    public async Task UpdateGroqOptionsAsync(GroqOptions options)
    {
        var isInMemory = context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
        
        if (!isInMemory)
        {
            await context.Database.BeginTransactionAsync();
        }
        
        try
        {
            await UpsertAsync($"{GroqPrefix}ApiKey", options.ApiKey);
            await UpsertAsync($"{GroqPrefix}BaseUrl", options.BaseUrl);
            await UpsertAsync($"{GroqPrefix}Model", options.Model);
            await UpsertAsync($"{GroqPrefix}TimeoutSeconds", options.TimeoutSeconds.ToString());
            await UpsertAsync($"{GroqPrefix}MaxTokens", options.MaxTokens.ToString());
            
            if (!isInMemory)
            {
                await context.Database.CommitTransactionAsync();
            }
        }
        catch
        {
            if (!isInMemory)
            {
                await context.Database.RollbackTransactionAsync();
            }
            throw;
        }
    }

    private async Task<Dictionary<string, string>> GetSettingsByPrefixAsync(string prefix)
    {
        var settings = await context.ApplicationSettings
            .Where(s => s.Key.StartsWith(prefix))
            .ToListAsync();

        return settings.ToDictionary(s => s.Key, s => s.Value);
    }

    private async Task UpsertAsync(string key, string value)
    {
        var setting = await context.ApplicationSettings.FindAsync(key);
        if (setting is null)
        {
            context.ApplicationSettings.Add(new ApplicationSetting
            {
                Key = key,
                Value = value,
                UpdatedAt = DateTime.UtcNow
            });
        }
        else
        {
            setting.Value = value;
            setting.UpdatedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
    }

    private static string GetString(Dictionary<string, string> settings, string key, string defaultValue)
        => settings.TryGetValue(key, out var value) ? value : defaultValue;

    private static bool GetBool(Dictionary<string, string> settings, string key, bool defaultValue)
        => settings.TryGetValue(key, out var value) && bool.TryParse(value, out var result) ? result : defaultValue;

    private static int GetInt(Dictionary<string, string> settings, string key, int defaultValue)
        => settings.TryGetValue(key, out var value) && int.TryParse(value, out var result) ? result : defaultValue;
}

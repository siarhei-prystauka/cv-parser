using CvParser.Api.Data;
using CvParser.Api.Models.Options;
using CvParser.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CvParser.Api.Tests.Repositories;

/// <summary>
/// Tests for <see cref="SqlSettingsRepository"/>.
/// </summary>
public sealed class SqlSettingsRepositoryTests
{
    private static ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Test]
    public async Task GetSkillExtractionOptionsAsync_WithNoData_ReturnsDefaults()
    {
        await using var context = CreateInMemoryContext();
        var repository = new SqlSettingsRepository(context);

        var result = await repository.GetSkillExtractionOptionsAsync();

        Assert.That(result.LlmFallbackOnly, Is.False);
    }

    [Test]
    public async Task UpdateSkillExtractionOptionsAsync_WithNewOptions_PersistsChanges()
    {
        await using var context = CreateInMemoryContext();
        var repository = new SqlSettingsRepository(context);

        await repository.UpdateSkillExtractionOptionsAsync(new SkillExtractionOptions { LlmFallbackOnly = true });
        var result = await repository.GetSkillExtractionOptionsAsync();

        Assert.That(result.LlmFallbackOnly, Is.True);
    }

    [Test]
    public async Task GetGroqOptionsAsync_WithNoData_ReturnsDefaults()
    {
        await using var context = CreateInMemoryContext();
        var repository = new SqlSettingsRepository(context);

        var result = await repository.GetGroqOptionsAsync();

        Assert.That(result.BaseUrl, Is.EqualTo("https://api.groq.com/openai/v1/"));
        Assert.That(result.TimeoutSeconds, Is.EqualTo(30));
        Assert.That(result.MaxTokens, Is.EqualTo(1000));
    }

    [Test]
    public async Task UpdateGroqOptionsAsync_WithNewOptions_PersistsChanges()
    {
        await using var context = CreateInMemoryContext();
        var repository = new SqlSettingsRepository(context);

        var newOptions = new GroqOptions
        {
            ApiKey = "new-key",
            Model = "new-model",
            TimeoutSeconds = 60,
            MaxTokens = 2048,
            BaseUrl = "https://api.groq.com/openai/v1/"
        };
        await repository.UpdateGroqOptionsAsync(newOptions);
        var result = await repository.GetGroqOptionsAsync();

        Assert.That(result.ApiKey, Is.EqualTo("new-key"));
        Assert.That(result.Model, Is.EqualTo("new-model"));
        Assert.That(result.TimeoutSeconds, Is.EqualTo(60));
    }

    [Test]
    public async Task UpdateGroqOptionsAsync_CalledTwice_OverwritesPreviousValues()
    {
        await using var context = CreateInMemoryContext();
        var repository = new SqlSettingsRepository(context);

        await repository.UpdateGroqOptionsAsync(new GroqOptions { ApiKey = "key-1", Model = "model-1", BaseUrl = "https://api.groq.com/openai/v1/" });
        await repository.UpdateGroqOptionsAsync(new GroqOptions { ApiKey = "key-2", Model = "model-2", BaseUrl = "https://api.groq.com/openai/v1/" });
        var result = await repository.GetGroqOptionsAsync();

        Assert.That(result.ApiKey, Is.EqualTo("key-2"));
        Assert.That(result.Model, Is.EqualTo("model-2"));
    }
}

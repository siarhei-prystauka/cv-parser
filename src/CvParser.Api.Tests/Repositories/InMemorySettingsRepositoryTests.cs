using CvParser.Api.Models.Options;
using CvParser.Api.Repositories;
using Microsoft.Extensions.Options;

namespace CvParser.Api.Tests.Repositories;

/// <summary>
/// Tests for <see cref="InMemorySettingsRepository"/>.
/// </summary>
public sealed class InMemorySettingsRepositoryTests
{
    [Test]
    public async Task GetSkillExtractionOptionsAsync_OnInitialization_ReturnsInitialValues()
    {
        var initialOptions = new SkillExtractionOptions { LlmFallbackOnly = true };
        var groqOptions = new GroqOptions { ApiKey = "test-key" };
        var repository = new InMemorySettingsRepository(
            Options.Create(initialOptions),
            Options.Create(groqOptions)
        );

        var result = await repository.GetSkillExtractionOptionsAsync();

        Assert.That(result.LlmFallbackOnly, Is.True);
    }

    [Test]
    public async Task GetGroqOptionsAsync_OnInitialization_ReturnsInitialValues()
    {
        var skillOptions = new SkillExtractionOptions { LlmFallbackOnly = false };
        var initialGroqOptions = new GroqOptions 
        { 
            ApiKey = "test-key",
            Model = "test-model",
            TimeoutSeconds = 60
        };
        var repository = new InMemorySettingsRepository(
            Options.Create(skillOptions),
            Options.Create(initialGroqOptions)
        );

        var result = await repository.GetGroqOptionsAsync();

        Assert.That(result.ApiKey, Is.EqualTo("test-key"));
        Assert.That(result.Model, Is.EqualTo("test-model"));
        Assert.That(result.TimeoutSeconds, Is.EqualTo(60));
    }

    [Test]
    public async Task UpdateSkillExtractionOptionsAsync_WithNewOptions_PersistsChanges()
    {
        var initialOptions = new SkillExtractionOptions { LlmFallbackOnly = false };
        var groqOptions = new GroqOptions { ApiKey = "test-key" };
        var repository = new InMemorySettingsRepository(
            Options.Create(initialOptions),
            Options.Create(groqOptions)
        );

        var newOptions = new SkillExtractionOptions { LlmFallbackOnly = true };
        await repository.UpdateSkillExtractionOptionsAsync(newOptions);

        var result = await repository.GetSkillExtractionOptionsAsync();
        Assert.That(result.LlmFallbackOnly, Is.True);
    }

    [Test]
    public async Task UpdateGroqOptionsAsync_WithNewOptions_PersistsChanges()
    {
        var skillOptions = new SkillExtractionOptions { LlmFallbackOnly = false };
        var initialGroqOptions = new GroqOptions { ApiKey = "old-key" };
        var repository = new InMemorySettingsRepository(
            Options.Create(skillOptions),
            Options.Create(initialGroqOptions)
        );

        var newGroqOptions = new GroqOptions 
        { 
            ApiKey = "new-key",
            Model = "new-model",
            TimeoutSeconds = 90
        };
        await repository.UpdateGroqOptionsAsync(newGroqOptions);

        var result = await repository.GetGroqOptionsAsync();
        Assert.That(result.ApiKey, Is.EqualTo("new-key"));
        Assert.That(result.Model, Is.EqualTo("new-model"));
        Assert.That(result.TimeoutSeconds, Is.EqualTo(90));
    }

    [Test]
    public async Task ConcurrentUpdates_WithMultipleThreads_MaintainsDataConsistency()
    {
        var skillOptions = new SkillExtractionOptions { LlmFallbackOnly = false };
        var groqOptions = new GroqOptions { ApiKey = "initial-key" };
        var repository = new InMemorySettingsRepository(
            Options.Create(skillOptions),
            Options.Create(groqOptions)
        );

        const int threadCount = 50;
        var tasks = new Task[threadCount];

        for (var i = 0; i < threadCount; i++)
        {
            var index = i;
            tasks[i] = Task.Run(async () =>
            {
                if (index % 2 == 0)
                {
                    var opts = new SkillExtractionOptions { LlmFallbackOnly = index % 4 == 0 };
                    await repository.UpdateSkillExtractionOptionsAsync(opts);
                }
                else
                {
                    var opts = new GroqOptions { ApiKey = $"key-{index}" };
                    await repository.UpdateGroqOptionsAsync(opts);
                }

                await repository.GetSkillExtractionOptionsAsync();
                await repository.GetGroqOptionsAsync();
            });
        }

        await Task.WhenAll(tasks);

        var finalSkillOptions = await repository.GetSkillExtractionOptionsAsync();
        var finalGroqOptions = await repository.GetGroqOptionsAsync();

        Assert.That(finalSkillOptions, Is.Not.Null);
        Assert.That(finalGroqOptions, Is.Not.Null);
        Assert.That(finalGroqOptions.ApiKey, Does.StartWith("key-"));
    }
}

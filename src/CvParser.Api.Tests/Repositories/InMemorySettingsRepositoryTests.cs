using CvParser.Api.Models;
using CvParser.Api.Models.Options;
using CvParser.Api.Repositories;
using Microsoft.Extensions.Options;

namespace CvParser.Api.Tests.Repositories;

/// <summary>
/// Tests for <see cref="InMemorySettingsRepository"/>.
/// </summary>
public sealed class InMemorySettingsRepositoryTests
{
    private static InMemorySettingsRepository CreateRepository(bool llmFallbackOnly = false, string model = "llama-3.3-70b-versatile")
    {
        return new InMemorySettingsRepository(
            Options.Create(new SkillExtractionOptions { LlmFallbackOnly = llmFallbackOnly }),
            Options.Create(new GroqOptions { Model = model })
        );
    }

    [Test]
    public async Task GetAsync_OnInitialization_ReturnsInitialValues()
    {
        var repository = CreateRepository(llmFallbackOnly: true, model: "llama-3.1-8b-instant");

        var result = await repository.GetAsync();

        Assert.That(result.LlmFallbackOnly, Is.True);
        Assert.That(result.LlmModel, Is.EqualTo("llama-3.1-8b-instant"));
    }

    [Test]
    public async Task UpdateAsync_WithNewValues_PersistsChanges()
    {
        var repository = CreateRepository();

        await repository.UpdateAsync(new ApplicationSetting { LlmFallbackOnly = true, LlmModel = "llama-3.1-8b-instant" });
        var result = await repository.GetAsync();

        Assert.That(result.LlmFallbackOnly, Is.True);
        Assert.That(result.LlmModel, Is.EqualTo("llama-3.1-8b-instant"));
    }

    [Test]
    public async Task UpdateAsync_CalledTwice_OverwritesPreviousValues()
    {
        var repository = CreateRepository();

        await repository.UpdateAsync(new ApplicationSetting { LlmFallbackOnly = false, LlmModel = "model-1" });
        await repository.UpdateAsync(new ApplicationSetting { LlmFallbackOnly = true, LlmModel = "model-2" });
        var result = await repository.GetAsync();

        Assert.That(result.LlmFallbackOnly, Is.True);
        Assert.That(result.LlmModel, Is.EqualTo("model-2"));
    }

    [Test]
    public async Task ConcurrentUpdates_WithMultipleThreads_MaintainsDataConsistency()
    {
        var repository = CreateRepository();
        const int threadCount = 50;
        var tasks = new Task[threadCount];

        for (var i = 0; i < threadCount; i++)
        {
            var index = i;
            tasks[i] = Task.Run(async () =>
            {
                await repository.UpdateAsync(new ApplicationSetting
                {
                    LlmFallbackOnly = index % 2 == 0,
                    LlmModel = $"model-{index}"
                });
                await repository.GetAsync();
            });
        }

        await Task.WhenAll(tasks);

        var finalSetting = await repository.GetAsync();
        Assert.That(finalSetting, Is.Not.Null);
        Assert.That(finalSetting.LlmModel, Does.StartWith("model-"));
    }
}

using CvParser.Api.Data;
using CvParser.Api.Models;
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
    public async Task GetAsync_WithNoData_ReturnsDefaults()
    {
        await using var context = CreateInMemoryContext();
        var repository = new SqlSettingsRepository(context);

        var result = await repository.GetAsync();

        Assert.That(result.LlmFallbackOnly, Is.False);
        Assert.That(result.LlmModel, Is.EqualTo("llama-3.3-70b-versatile"));
    }

    [Test]
    public async Task UpdateAsync_WithNewValues_PersistsChanges()
    {
        await using var context = CreateInMemoryContext();
        var repository = new SqlSettingsRepository(context);

        await repository.UpdateAsync(new ApplicationSetting { LlmFallbackOnly = true, LlmModel = "llama-3.1-8b-instant" });
        var result = await repository.GetAsync();

        Assert.That(result.LlmFallbackOnly, Is.True);
        Assert.That(result.LlmModel, Is.EqualTo("llama-3.1-8b-instant"));
    }

    [Test]
    public async Task UpdateAsync_CalledTwice_OverwritesPreviousValues()
    {
        await using var context = CreateInMemoryContext();
        var repository = new SqlSettingsRepository(context);

        await repository.UpdateAsync(new ApplicationSetting { LlmFallbackOnly = false, LlmModel = "model-1" });
        await repository.UpdateAsync(new ApplicationSetting { LlmFallbackOnly = true, LlmModel = "model-2" });
        var result = await repository.GetAsync();

        Assert.That(result.LlmFallbackOnly, Is.True);
        Assert.That(result.LlmModel, Is.EqualTo("model-2"));
    }

    [Test]
    public async Task UpdateAsync_SetsUpdatedAt()
    {
        await using var context = CreateInMemoryContext();
        var repository = new SqlSettingsRepository(context);
        var before = DateTime.UtcNow;

        await repository.UpdateAsync(new ApplicationSetting { LlmFallbackOnly = false, LlmModel = "llama-3.3-70b-versatile" });
        var result = await context.ApplicationSettings.SingleAsync();

        Assert.That(result.UpdatedAt, Is.GreaterThanOrEqualTo(before));
    }
}

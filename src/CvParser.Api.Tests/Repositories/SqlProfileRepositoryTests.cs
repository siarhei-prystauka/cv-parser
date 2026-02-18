using CvParser.Api.Data;
using CvParser.Api.Models;
using CvParser.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CvParser.Api.Tests.Repositories;

/// <summary>
/// Tests for <see cref="SqlProfileRepository"/>.
/// </summary>
public sealed class SqlProfileRepositoryTests
{
    private static ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static List<EmployeeProfile> BuildSeedProfiles() =>
    [
        new EmployeeProfile { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Smith", DepartmentName = "Engineering", Skills = ["C#", "SQL"] },
        new EmployeeProfile { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Jones", DepartmentName = "Product", Skills = ["Roadmapping"] }
    ];

    [Test]
    public async Task GetAllAsync_WithSeededProfiles_ReturnsAllProfiles()
    {
        await using var context = CreateInMemoryContext();
        var profiles = BuildSeedProfiles();
        context.EmployeeProfiles.AddRange(profiles);
        await context.SaveChangesAsync();

        var repository = new SqlProfileRepository(context);
        var result = await repository.GetAllAsync();

        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_WithExistingId_ReturnsProfile()
    {
        await using var context = CreateInMemoryContext();
        var profile = BuildSeedProfiles()[0];
        context.EmployeeProfiles.Add(profile);
        await context.SaveChangesAsync();

        var repository = new SqlProfileRepository(context);
        var result = await repository.GetByIdAsync(profile.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.FirstName, Is.EqualTo(profile.FirstName));
    }

    [Test]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        await using var context = CreateInMemoryContext();
        var repository = new SqlProfileRepository(context);

        var result = await repository.GetByIdAsync(Guid.NewGuid());

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task UpdateSkillsAsync_WithExistingId_UpdatesAndReturnsProfile()
    {
        await using var context = CreateInMemoryContext();
        var profile = BuildSeedProfiles()[0];
        context.EmployeeProfiles.Add(profile);
        await context.SaveChangesAsync();

        var repository = new SqlProfileRepository(context);
        var newSkills = new List<string> { "Python", "Docker" };
        var result = await repository.UpdateSkillsAsync(profile.Id, newSkills);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Skills, Is.EquivalentTo(newSkills));
    }

    [Test]
    public async Task UpdateSkillsAsync_WithNonExistentId_ReturnsNull()
    {
        await using var context = CreateInMemoryContext();
        var repository = new SqlProfileRepository(context);

        var result = await repository.UpdateSkillsAsync(Guid.NewGuid(), ["C#"]);

        Assert.That(result, Is.Null);
    }
}

using CvParser.Api.Repositories;

namespace CvParser.Api.Tests.Repositories;

/// <summary>
/// Tests for <see cref="InMemoryProfileRepository"/>.
/// </summary>
public sealed class InMemoryProfileRepositoryTests
{
    /// <summary>
    /// Ensures seeded profiles are available for development.
    /// </summary>
    [Test]
    public void GetAll_OnInitialization_ReturnsSeededProfiles()
    {
        var repository = new InMemoryProfileRepository();

        var profiles = repository.GetAll();

        Assert.That(profiles, Is.Not.Empty);
        Assert.That(profiles.Count, Is.GreaterThanOrEqualTo(1));
    }
}

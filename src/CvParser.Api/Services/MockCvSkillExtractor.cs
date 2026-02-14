using System.Security.Cryptography;
using System.Text;

namespace CvParser.Api.Services;

/// <summary>
/// Provides deterministic placeholder skills for development previews.
/// </summary>
public sealed class MockCvSkillExtractor : ICvSkillExtractor
{
    private static readonly string[] SkillPool =
    [
        "Agile delivery",
        "API integration",
        "Communication",
        "Data visualization",
        "Documentation",
        "Leadership",
        "Project planning",
        "Quality assurance",
        "Stakeholder management",
        "Systems thinking",
        "Technical writing",
        "User research"
    ];

    /// <inheritdoc />
    public Task<IReadOnlyList<string>> ExtractSkillsAsync(Stream fileStream, string fileName, string contentType)
    {
        var seed = BuildSeed(fileStream, fileName);
        var random = new Random(seed);
        var selectionCount = random.Next(3, 6);
        var skills = SkillPool.OrderBy(_ => random.Next()).Take(selectionCount).ToList();

        return Task.FromResult<IReadOnlyList<string>>(skills);
    }

    /// <summary>
    /// Builds a repeatable seed from file metadata.
    /// </summary>
    private static int BuildSeed(Stream fileStream, string fileName)
    {
        var fileNameBytes = Encoding.UTF8.GetBytes(fileName);
        var lengthBytes = BitConverter.GetBytes(fileStream.Length);
        var hashInput = fileNameBytes.Concat(lengthBytes).ToArray();
        var hash = SHA256.HashData(hashInput);

        return BitConverter.ToInt32(hash, 0);
    }
}

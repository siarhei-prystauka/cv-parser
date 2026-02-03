using CvParser.Core.Services;
using System.Text;

namespace CvParser.Tests;

public class CvParserServiceTests
{
    [Fact]
    public async Task ParseCvAsync_WithValidStream_ReturnsDocumentWithSkills()
    {
        // Arrange
        var service = new CvParserService();
        var content = "Software Developer with 5 years of experience in C# and Python. Worked with Azure and Docker.";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var fileName = "test-cv.txt";

        // Act
        var result = await service.ParseCvAsync(stream, fileName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fileName, result.FileName);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotNull(result.Skills);
        Assert.True(result.Skills.Count > 0);
    }

    [Fact]
    public async Task ExtractSkillsAsync_WithCSharp_ExtractsCSharpSkill()
    {
        // Arrange
        var service = new CvParserService();
        var text = "Experienced C# developer";

        // Act
        var skills = await service.ExtractSkillsAsync(text);

        // Assert
        Assert.Contains(skills, s => s.Name == "C#");
    }

    [Fact]
    public async Task ExtractSkillsAsync_WithMultipleSkills_ExtractsAllSkills()
    {
        // Arrange
        var service = new CvParserService();
        var text = "Skilled in Python, JavaScript, SQL, and Docker";

        // Act
        var skills = await service.ExtractSkillsAsync(text);

        // Assert
        Assert.True(skills.Count >= 3);
        Assert.Contains(skills, s => s.Name == "Python");
        Assert.Contains(skills, s => s.Name == "JavaScript");
        Assert.Contains(skills, s => s.Name == "SQL");
    }
}

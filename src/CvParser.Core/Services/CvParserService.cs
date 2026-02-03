using System.Text;
using CvParser.Core.Interfaces;
using CvParser.Core.Models;

namespace CvParser.Core.Services;

/// <summary>
/// Basic implementation of CV parser service
/// </summary>
public class CvParserService : ICvParserService
{
    public async Task<CvDocument> ParseCvAsync(Stream documentStream, string fileName)
    {
        // Read the document content
        using var reader = new StreamReader(documentStream);
        var content = await reader.ReadToEndAsync();
        
        var document = new CvDocument
        {
            FileName = fileName,
            RawText = content,
            ParsedAt = DateTime.UtcNow
        };
        
        // Extract basic information (placeholder implementation)
        document.Skills = await ExtractSkillsAsync(content);
        
        return document;
    }
    
    public Task<List<Skill>> ExtractSkillsAsync(string cvText)
    {
        // Placeholder implementation - in a real application, this would use
        // NLP or ML models to extract skills from the text
        var skills = new List<Skill>();
        
        // Simple keyword matching as a starting point
        var commonSkills = new Dictionary<string, string>
        {
            { "C#", "Programming" },
            { "Python", "Programming" },
            { "JavaScript", "Programming" },
            { "Java", "Programming" },
            { "SQL", "Database" },
            { "Azure", "Cloud" },
            { "AWS", "Cloud" },
            { "Docker", "DevOps" },
            { "Kubernetes", "DevOps" }
        };
        
        foreach (var (skillName, category) in commonSkills)
        {
            if (cvText.Contains(skillName, StringComparison.OrdinalIgnoreCase))
            {
                skills.Add(new Skill
                {
                    Name = skillName,
                    Category = category,
                    Level = ProficiencyLevel.Intermediate,
                    YearsOfExperience = 0
                });
            }
        }
        
        return Task.FromResult(skills);
    }
}

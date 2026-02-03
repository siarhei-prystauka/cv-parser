namespace CvParser.Core.Models;

/// <summary>
/// Represents a skill extracted from a CV or IFU document
/// </summary>
public class Skill
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public ProficiencyLevel Level { get; set; }
}

public enum ProficiencyLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}

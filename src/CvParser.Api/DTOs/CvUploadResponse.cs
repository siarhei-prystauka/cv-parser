namespace CvParser.Api.DTOs;

/// <summary>
/// Response DTO for uploaded CV document
/// </summary>
public class CvUploadResponse
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public List<SkillDto> ExtractedSkills { get; set; } = new();
    public DateTime UploadedAt { get; set; }
}

public class SkillDto
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string Level { get; set; } = string.Empty;
}

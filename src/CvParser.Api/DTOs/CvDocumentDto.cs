namespace CvParser.Api.DTOs;

/// <summary>
/// Response DTO for CV document details
/// </summary>
public class CvDocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<SkillDto> Skills { get; set; } = new();
    public DateTime UploadedAt { get; set; }
    public DateTime? ParsedAt { get; set; }
}

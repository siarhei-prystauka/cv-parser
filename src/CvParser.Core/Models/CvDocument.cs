namespace CvParser.Core.Models;

/// <summary>
/// Represents a parsed CV document with extracted information
/// </summary>
public class CvDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public List<Skill> Skills { get; set; } = new();
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ParsedAt { get; set; }
    public string RawText { get; set; } = string.Empty;
}

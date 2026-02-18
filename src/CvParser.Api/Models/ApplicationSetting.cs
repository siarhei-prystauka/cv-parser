namespace CvParser.Api.Models;

/// <summary>
/// Persisted application settings stored as a singleton row.
/// </summary>
public sealed class ApplicationSetting
{
    public int Id { get; set; }
    public bool LlmFallbackOnly { get; set; }
    public required string LlmModel { get; set; }
    public DateTime UpdatedAt { get; set; }
}

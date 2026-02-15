using System.ComponentModel.DataAnnotations;

namespace CvParser.Api.Models.Requests;

/// <summary>
/// Payload for updating application settings.
/// </summary>
public sealed class UpdateSettingsRequest
{
    [Required]
    public required SkillExtractionSettingsRequest SkillExtraction { get; set; }
    
    [Required]
    public required LlmSettingsRequest Llm { get; set; }
}

public sealed class SkillExtractionSettingsRequest
{
    public bool LlmFallbackOnly { get; set; }
}

public sealed class LlmSettingsRequest
{
    [Required]
    [RegularExpression(@"\S+")]
    public required string Model { get; set; }
}

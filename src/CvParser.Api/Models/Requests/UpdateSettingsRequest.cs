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

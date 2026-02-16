using System.ComponentModel.DataAnnotations;

namespace CvParser.Api.Models.Requests;

public sealed class LlmSettingsRequest
{
    [Required]
    [RegularExpression(@"\S+")]
    public required string Model { get; set; }
}

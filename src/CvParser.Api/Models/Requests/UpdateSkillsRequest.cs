using System.ComponentModel.DataAnnotations;

namespace CvParser.Api.Models.Requests;

/// <summary>
/// Payload for updating a profile's skill list.
/// </summary>
public sealed class UpdateSkillsRequest
{
    [Required]
    public required List<string> Skills { get; set; }
}

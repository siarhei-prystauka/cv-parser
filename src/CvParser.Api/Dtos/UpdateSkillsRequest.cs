using System.ComponentModel.DataAnnotations;

namespace CvParser.Api.Dtos;

/// <summary>
/// Payload for updating a profile skill list.
/// </summary>
public sealed class UpdateSkillsRequest
{
    /// <summary>
    /// Gets or sets the skills to store for the profile.
    /// </summary>
    [Required]
    public required List<string> Skills { get; set; }
}

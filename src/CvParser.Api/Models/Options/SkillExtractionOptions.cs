using System.ComponentModel.DataAnnotations;

namespace CvParser.Api.Models.Options;

public sealed class SkillExtractionOptions
{
    public const string SectionName = "SkillExtraction";
    
    public bool LlmFallbackOnly { get; init; }
}

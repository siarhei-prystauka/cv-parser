using System.ComponentModel.DataAnnotations;

namespace CvParser.Api.Models.Options;

public sealed class GroqOptions
{
    public const string SectionName = "Groq";
    
    [Required(AllowEmptyStrings = false)]
    [RegularExpression(@"\S+")]
    public string ApiKey { get; init; } = string.Empty;
    
    [Url]
    public string BaseUrl { get; init; } = "https://api.groq.com/openai/v1/";
    
    [Required]
    [RegularExpression(@"\S+")]
    public string Model { get; init; } = "llama-3.1-8b-instant";
    
    [Range(1, 300)]
    public int TimeoutSeconds { get; init; } = 30;
    
    [Range(100, 10000)]
    public int MaxTokens { get; init; } = 1000;
}

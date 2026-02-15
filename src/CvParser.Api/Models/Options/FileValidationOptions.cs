using System.ComponentModel.DataAnnotations;

namespace CvParser.Api.Models.Options;

public sealed class FileValidationOptions
{
    public const string SectionName = "FileValidation";
    
    [Range(1024, 104857600)] // 1 KB to 100 MB
    public long MaxFileSizeBytes { get; init; } = 10485760; // 10 MB
    
    [Required]
    [MinLength(1)]
    public string[] SupportedContentTypes { get; init; } = ["application/pdf"];
}

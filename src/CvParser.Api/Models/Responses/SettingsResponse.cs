namespace CvParser.Api.Models.Responses;

/// <summary>
/// Current application settings configuration.
/// </summary>
public sealed record SettingsResponse(
    SkillExtractionSettings SkillExtraction,
    LlmSettings Llm
);

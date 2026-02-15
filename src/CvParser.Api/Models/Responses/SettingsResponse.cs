namespace CvParser.Api.Models.Responses;

/// <summary>
/// Current application settings configuration.
/// </summary>
public sealed record SettingsResponse(
    SkillExtractionSettings SkillExtraction,
    LlmSettings Llm
);

public sealed record SkillExtractionSettings(
    bool LlmFallbackOnly
);

public sealed record LlmSettings(
    string Model,
    string[] AvailableModels
);

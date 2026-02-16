namespace CvParser.Api.Models.Responses;

public sealed record LlmSettings(
    string Model,
    string[] AvailableModels
);

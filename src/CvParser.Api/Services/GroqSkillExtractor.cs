using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using CvParser.Api.Models.Options;
using Microsoft.Extensions.Options;

namespace CvParser.Api.Services;

/// <summary>
/// Extracts skills via the Groq LLM API (OpenAI-compatible).
/// </summary>
public class GroqSkillExtractor : ILlmSkillExtractor
{
    private readonly HttpClient _httpClient;
    private readonly GroqOptions _options;
    private readonly ILogger<GroqSkillExtractor> _logger;

    private const string SystemPrompt = @"You are a CV skill extraction assistant. Extract technical skills from the provided CV text and return them as a JSON array of strings. Focus on hard skills like programming languages, frameworks, tools, databases, cloud platforms, and certifications. Exclude soft skills and general terms. Return only the JSON object in this exact format: {""skills"": [""skill1"", ""skill2"", ""skill3""]}";

    public GroqSkillExtractor(
        HttpClient httpClient,
        IOptions<GroqOptions> options,
        ILogger<GroqSkillExtractor> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IEnumerable<string>> ExtractSkillsAsync(string cvText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cvText))
        {
            _logger.LogWarning("Empty CV text provided to Groq extractor");
            return Enumerable.Empty<string>();
        }

        var apiKey = _options.ApiKey.Trim();

        _logger.LogDebug("Groq API key is configured.");

        try
        {
            var truncatedText = cvText.Length > 3000 ? cvText[..3000] : cvText;
            
            if (cvText.Length > 3000)
            {
                _logger.LogWarning("CV text truncated from {OriginalLength} to {TruncatedLength} characters for LLM processing",
                    cvText.Length, 3000);
            }

            var request = new GroqRequest
            {
                Model = _options.Model,
                Messages = new[]
                {
                    new GroqMessage { Role = "system", Content = SystemPrompt },
                    new GroqMessage { Role = "user", Content = $"CV Text:\n{truncatedText}" }
                },
                ResponseFormat = new { type = "json_object" },
                MaxTokens = _options.MaxTokens,
                Temperature = 0.1
            };

            var requestJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
            {
                Content = new StringContent(requestJson, System.Text.Encoding.UTF8, "application/json")
            };
            
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var fullUrl = $"{_httpClient.BaseAddress}chat/completions";
            _logger.LogInformation("Sending request to Groq API: {Url} with model {Model}", fullUrl, _options.Model);

            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var correlationId = Guid.NewGuid().ToString("D");
                _logger.LogError("Groq API returned {StatusCode} with CorrelationId {CorrelationId}: {Error}", response.StatusCode, correlationId, errorContent);
                throw new InvalidOperationException($"Groq API error. StatusCode: {response.StatusCode}, CorrelationId: {correlationId}");
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var groqResponse = JsonSerializer.Deserialize<GroqResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (groqResponse?.Choices is null || groqResponse.Choices.Length == 0)
            {
                _logger.LogWarning("Groq API returned no choices");
                return Enumerable.Empty<string>();
            }

            var messageContent = groqResponse.Choices[0].Message?.Content;
            if (string.IsNullOrWhiteSpace(messageContent))
            {
                _logger.LogWarning("Groq API returned empty message content");
                return Enumerable.Empty<string>();
            }

            var skillsResponse = JsonSerializer.Deserialize<SkillsResponse>(messageContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var skills = skillsResponse?.Skills ?? Enumerable.Empty<string>();
            _logger.LogInformation("Groq extracted {Count} skills", skills.Count());

            return skills.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling Groq API");
            throw new InvalidOperationException("Failed to call Groq API", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse Groq API response");
            throw new InvalidOperationException("Failed to parse Groq API response", ex);
        }
    }

    private record GroqRequest
    {
        public required string Model { get; init; }
        public required GroqMessage[] Messages { get; init; }
        public object? ResponseFormat { get; init; }
        public int MaxTokens { get; init; }
        public double Temperature { get; init; }
    }

    private record GroqMessage
    {
        public required string Role { get; init; }
        public required string Content { get; init; }
    }

    private record GroqResponse
    {
        public GroqChoice[]? Choices { get; init; }
    }

    private record GroqChoice
    {
        public GroqMessage? Message { get; init; }
    }

    private record SkillsResponse
    {
        public IEnumerable<string>? Skills { get; init; }
    }
}

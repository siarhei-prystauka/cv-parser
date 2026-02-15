using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CvParser.Api.Services;

/// <summary>
/// Groq LLM skill extractor using the OpenAI-compatible API.
/// </summary>
public class GroqSkillExtractor : ILlmSkillExtractor
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GroqSkillExtractor> _logger;

    private const string SystemPrompt = @"You are a CV skill extraction assistant. Extract technical skills from the provided CV text and return them as a JSON array of strings. Focus on hard skills like programming languages, frameworks, tools, databases, cloud platforms, and certifications. Exclude soft skills and general terms. Return only the JSON object in this exact format: {""skills"": [""skill1"", ""skill2"", ""skill3""]}";

    public GroqSkillExtractor(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GroqSkillExtractor> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Extracts skills from CV text using Groq's Llama model.
    /// </summary>
    public async Task<IEnumerable<string>> ExtractSkillsAsync(string cvText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cvText))
        {
            _logger.LogWarning("Empty CV text provided to Groq extractor");
            return Enumerable.Empty<string>();
        }

        var apiKey = _configuration["Groq:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Groq API key is not configured. Set the Groq:ApiKey configuration value.");
        }

        // Trim whitespace that might have been accidentally added
        apiKey = apiKey.Trim();
        
        // Debug: Confirm that an API key is configured without logging any part of it
        _logger.LogDebug("Groq API key is configured.");

        var model = _configuration["Groq:Model"] ?? "llama-3.1-8b-instant";
        
        var maxTokensConfig = _configuration["Groq:MaxTokens"];
        var maxTokens = 1000;
        if (maxTokensConfig is not null && !int.TryParse(maxTokensConfig, out maxTokens))
        {
            _logger.LogWarning(
                "Invalid Groq:MaxTokens configuration value: {Value}. Falling back to default {DefaultMaxTokens}.",
                maxTokensConfig,
                1000);
            maxTokens = 1000;
        }

        try
        {
            // Truncate CV text if too long (keep first 3000 chars to stay within token limits)
            var truncatedText = cvText.Length > 3000 ? cvText[..3000] : cvText;
            
            if (cvText.Length > 3000)
            {
                _logger.LogWarning("CV text truncated from {OriginalLength} to {TruncatedLength} characters for LLM processing",
                    cvText.Length, 3000);
            }

            var request = new GroqRequest
            {
                Model = model,
                Messages = new[]
                {
                    new GroqMessage { Role = "system", Content = SystemPrompt },
                    new GroqMessage { Role = "user", Content = $"CV Text:\n{truncatedText}" }
                },
                ResponseFormat = new { type = "json_object" },
                MaxTokens = maxTokens,
                Temperature = 0.1 // Low temperature for consistent extraction
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
            _logger.LogInformation("Sending request to Groq API: {Url} with model {Model}", fullUrl, model);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Groq API returned {StatusCode}: {Error}", response.StatusCode, errorContent);
                throw new InvalidOperationException($"Groq API error: {response.StatusCode} - {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
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

            // Parse the JSON response to extract skills array
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

    /// <summary>
    /// Groq API request model.
    /// </summary>
    private record GroqRequest
    {
        public required string Model { get; init; }
        public required GroqMessage[] Messages { get; init; }
        public object? ResponseFormat { get; init; }
        public int MaxTokens { get; init; }
        public double Temperature { get; init; }
    }

    /// <summary>
    /// Groq API message model.
    /// </summary>
    private record GroqMessage
    {
        public required string Role { get; init; }
        public required string Content { get; init; }
    }

    /// <summary>
    /// Groq API response model.
    /// </summary>
    private record GroqResponse
    {
        public GroqChoice[]? Choices { get; init; }
    }

    /// <summary>
    /// Groq API choice model.
    /// </summary>
    private record GroqChoice
    {
        public GroqMessage? Message { get; init; }
    }

    /// <summary>
    /// Skills response from Groq (parsed from JSON content).
    /// </summary>
    private record SkillsResponse
    {
        public IEnumerable<string>? Skills { get; init; }
    }
}

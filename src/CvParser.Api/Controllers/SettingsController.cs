using System.Text.Json;
using CvParser.Api.Models.Options;
using CvParser.Api.Models.Requests;
using CvParser.Api.Models.Responses;
using CvParser.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CvParser.Api.Controllers;

/// <summary>
/// Manages application runtime configuration settings.
/// </summary>
[ApiController]
[Route("api/v1/Settings")]
[Produces("application/json")]
public class SettingsController : ControllerBase
{
    private readonly ISettingsRepository _repository;
    private readonly IWebHostEnvironment _environment;

    private static readonly string[] AvailableModels =
    [
        "llama-3.3-70b-versatile",
        "llama-3.1-70b-versatile",
        "llama-3.1-8b-instant",
        "llama3-groq-70b-8192-tool-use-preview",
        "llama3-groq-8b-8192-tool-use-preview",
        "mixtral-8x7b-32768",
        "gemma2-9b-it",
        "gemma-7b-it"
    ];

    public SettingsController(ISettingsRepository repository, IWebHostEnvironment environment)
    {
        _repository = repository;
        _environment = environment;
    }

    /// <summary>
    /// Retrieves current application settings.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SettingsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SettingsResponse>> GetSettings()
    {
        var skillExtractionOptions = await _repository.GetSkillExtractionOptionsAsync();
        var groqOptions = await _repository.GetGroqOptionsAsync();

        var response = new SettingsResponse(
            new SkillExtractionSettings(skillExtractionOptions.LlmFallbackOnly),
            new LlmSettings(groqOptions.Model, AvailableModels)
        );

        return Ok(response);
    }

    /// <summary>
    /// Updates application settings.
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(SettingsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SettingsResponse>> UpdateSettings([FromBody] UpdateSettingsRequest request)
    {
        if (!AvailableModels.Contains(request.Llm.Model))
        {
            ModelState.AddModelError(nameof(request.Llm.Model), $"Model must be one of: {string.Join(", ", AvailableModels)}");
            return ValidationProblem(ModelState);
        }

        var skillExtractionOptions = new SkillExtractionOptions
        {
            LlmFallbackOnly = request.SkillExtraction.LlmFallbackOnly
        };

        var currentGroqOptions = await _repository.GetGroqOptionsAsync();
        var groqOptions = new GroqOptions
        {
            ApiKey = currentGroqOptions.ApiKey,
            BaseUrl = currentGroqOptions.BaseUrl,
            Model = request.Llm.Model,
            TimeoutSeconds = currentGroqOptions.TimeoutSeconds,
            MaxTokens = currentGroqOptions.MaxTokens
        };

        await _repository.UpdateSkillExtractionOptionsAsync(skillExtractionOptions);
        await _repository.UpdateGroqOptionsAsync(groqOptions);

        var response = new SettingsResponse(
            new SkillExtractionSettings(skillExtractionOptions.LlmFallbackOnly),
            new LlmSettings(groqOptions.Model, AvailableModels)
        );

        return Ok(response);
    }

    /// <summary>
    /// Retrieves the skills taxonomy data.
    /// </summary>
    [HttpGet("taxonomy")]
    [ProducesResponseType(typeof(TaxonomyResponse), StatusCodes.Status200OK)]
    public ActionResult<TaxonomyResponse> GetTaxonomy()
    {
        var taxonomyPath = Path.Combine(_environment.ContentRootPath, "Data", "skills-taxonomy.json");

        if (!System.IO.File.Exists(taxonomyPath))
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: $"Skills taxonomy file not found at: {taxonomyPath}"
            );
        }

        var json = System.IO.File.ReadAllText(taxonomyPath);
        var taxonomy = JsonSerializer.Deserialize<TaxonomyJson>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (taxonomy is null || taxonomy.Skills is null)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: "Failed to deserialize skills taxonomy"
            );
        }

        var skills = taxonomy.Skills
            .Select(s => new TaxonomySkill(s.Name, s.Category, s.Aliases))
            .ToList();

        var response = new TaxonomyResponse(skills);
        return Ok(response);
    }

    private sealed record TaxonomyJson
    {
        public required SkillJson[] Skills { get; init; }
    }

    private sealed record SkillJson
    {
        public required string Name { get; init; }
        public required string Category { get; init; }
        public required string[] Aliases { get; init; }
    }
}

using CvParser.Api.Models.Options;
using CvParser.Api.Models.Requests;
using CvParser.Api.Models.Responses;
using CvParser.Api.Repositories;
using CvParser.Api.Services;
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
    private readonly ITaxonomyService _taxonomyService;

    private static readonly string[] AvailableModels =
    [
        "llama-3.3-70b-versatile",
        "llama-3.1-8b-instant"
    ];

    public SettingsController(ISettingsRepository repository, ITaxonomyService taxonomyService)
    {
        _repository = repository;
        _taxonomyService = taxonomyService;
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
        var skills = _taxonomyService.GetTaxonomy();
        var response = new TaxonomyResponse(skills);
        return Ok(response);
    }
}

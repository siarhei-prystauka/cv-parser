using CvParser.Api.Converters;
using CvParser.Api.Models;
using CvParser.Api.Models.Requests;
using CvParser.Api.Models.Responses;
using CvParser.Api.Repositories;
using CvParser.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CvParser.Api.Controllers;

/// <summary>
/// Manages employee profile operations including CV upload and skill extraction.
/// </summary>
[ApiController]
[Route("api/v1/Profiles")]
[Produces("application/json")]
public class ProfilesController : ControllerBase
{
    private readonly IProfileRepository _repository;
    private readonly ICvSkillExtractor _extractor;
    private readonly IProfileConverter _converter;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the ProfilesController.
    /// </summary>
    public ProfilesController(
        IProfileRepository repository,
        ICvSkillExtractor extractor,
        IProfileConverter converter,
        IConfiguration configuration)
    {
        _repository = repository;
        _extractor = extractor;
        _converter = converter;
        _configuration = configuration;
    }

    /// <summary>
    /// Retrieves all employee profiles.
    /// </summary>
    /// <returns>A list of profile summaries.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProfileSummary>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<ProfileSummary>> GetProfiles()
    {
        var profiles = _repository.GetAll().Select(_converter.ToSummary);
        return Ok(profiles);
    }

    /// <summary>
    /// Retrieves a specific employee profile by ID.
    /// </summary>
    /// <param name="id">The profile identifier.</param>
    /// <returns>The profile details.</returns>
    /// <response code="200">Returns the profile details.</response>
    /// <response code="404">If the profile is not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProfileDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProfileDetail> GetProfile(Guid id)
    {
        var profile = _repository.GetById(id);
        if (profile is null)
        {
            return NotFound();
        }

        return Ok(_converter.ToDetail(profile));
    }

    /// <summary>
    /// Previews skills extracted from an uploaded CV without saving.
    /// </summary>
    /// <param name="id">The profile identifier.</param>
    /// <param name="cvFile">The CV file (PDF only).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Extracted skills for preview.</returns>
    /// <response code="200">Returns extracted skills.</response>
    /// <response code="400">If the file is invalid, unsupported format, or exceeds size limit.</response>
    /// <response code="404">If the profile is not found.</response>
    [HttpPost("{id:guid}/cv/preview")]
    [ProducesResponseType(typeof(CvPreviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CvPreviewResponse>> PreviewCvSkills(
        Guid id,
        IFormFile cvFile,
        CancellationToken cancellationToken)
    {
        var profile = _repository.GetById(id);
        if (profile is null)
        {
            return NotFound();
        }

        var validationResult = ValidateCvFile(cvFile);
        if (validationResult is not null)
        {
            return validationResult;
        }

        await using var fileStream = cvFile.OpenReadStream();
        var skills = await _extractor.ExtractSkillsAsync(fileStream, cvFile.FileName, cvFile.ContentType, cancellationToken);
        var response = new CvPreviewResponse(cvFile.FileName, skills);

        return Ok(response);
    }

    /// <summary>
    /// Updates the skills for a profile after user confirmation.
    /// </summary>
    /// <param name="id">The profile identifier.</param>
    /// <param name="request">The skills to save.</param>
    /// <returns>The updated profile.</returns>
    /// <response code="200">Returns the updated profile.</response>
    /// <response code="400">If the skills payload is invalid.</response>
    /// <response code="404">If the profile is not found.</response>
    [HttpPut("{id:guid}/skills")]
    [ProducesResponseType(typeof(ProfileDetail), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ProfileDetail> UpdateSkills(Guid id, [FromBody] UpdateSkillsRequest request)
    {
        var validationResult = ValidateSkills(request);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var normalized = NormalizeSkills(request.Skills);
        var updated = _repository.UpdateSkills(id, normalized);
        if (updated is null)
        {
            return NotFound();
        }

        return Ok(_converter.ToDetail(updated));
    }

    /// <summary>
    /// Validates that the provided file is a non-empty PDF within size limits.
    /// </summary>
    private ActionResult? ValidateCvFile(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            ModelState.AddModelError("cvFile", "A CV file is required.");
            return ValidationProblem(ModelState);
        }

        // Check file size (10 MB default)
        var maxFileSizeBytes = _configuration.GetValue<long>("FileValidation:MaxFileSizeBytes", 10485760);
        if (file.Length > maxFileSizeBytes)
        {
            var maxSizeMb = maxFileSizeBytes / 1024.0 / 1024.0;
            ModelState.AddModelError("cvFile", $"File size exceeds the maximum allowed size of {maxSizeMb:F1} MB.");
            return ValidationProblem(ModelState);
        }

        // Check content type (PDF only for now)
        var extension = Path.GetExtension(file.FileName);
        var isPdf = string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase);

        if (!isPdf)
        {
            ModelState.AddModelError("cvFile", "Only PDF file format is supported.");
            return ValidationProblem(ModelState);
        }

        return null;
    }

    /// <summary>
    /// Validates the skills update payload.
    /// </summary>
    private ActionResult? ValidateSkills(UpdateSkillsRequest request)
    {
        if (request.Skills is null || request.Skills.Count == 0)
        {
            ModelState.AddModelError("skills", "At least one skill is required.");
            return ValidationProblem(ModelState);
        }

        if (request.Skills.Any(skill => string.IsNullOrWhiteSpace(skill)))
        {
            ModelState.AddModelError("skills", "Skills cannot be blank.");
            return ValidationProblem(ModelState);
        }

        return null;
    }

    /// <summary>
    /// Normalizes skills for consistent storage.
    /// </summary>
    private static IReadOnlyList<string> NormalizeSkills(IEnumerable<string> skills)
    {
        return skills
            .Where(skill => !string.IsNullOrWhiteSpace(skill))
            .Select(skill => skill.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

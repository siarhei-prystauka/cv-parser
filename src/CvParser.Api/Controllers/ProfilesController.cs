using CvParser.Api.Converters;
using CvParser.Api.Models;
using CvParser.Api.Models.Options;
using CvParser.Api.Models.Requests;
using CvParser.Api.Models.Responses;
using CvParser.Api.Repositories;
using CvParser.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CvParser.Api.Controllers;

/// <summary>
/// Manages employee profiles, CV upload, and skill extraction.
/// </summary>
[ApiController]
[Route("api/v1/Profiles")]
[Produces("application/json")]
public class ProfilesController : ControllerBase
{
    private readonly IProfileRepository _repository;
    private readonly ICvSkillExtractor _extractor;
    private readonly IProfileConverter _converter;
    private readonly FileValidationOptions _fileValidationOptions;

    public ProfilesController(
        IProfileRepository repository,
        ICvSkillExtractor extractor,
        IProfileConverter converter,
        IOptions<FileValidationOptions> fileValidationOptions)
    {
        _repository = repository;
        _extractor = extractor;
        _converter = converter;
        _fileValidationOptions = fileValidationOptions.Value;
    }

    /// <summary>
    /// Retrieves all employee profiles.
    /// </summary>
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

    private ActionResult? ValidateCvFile(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            ModelState.AddModelError("cvFile", "A CV file is required.");
            return ValidationProblem(ModelState);
        }

        if (file.Length > _fileValidationOptions.MaxFileSizeBytes)
        {
            var maxSizeMb = _fileValidationOptions.MaxFileSizeBytes / 1024.0 / 1024.0;
            ModelState.AddModelError("cvFile", $"File size exceeds the maximum allowed size of {maxSizeMb:F1} MB.");
            return ValidationProblem(ModelState);
        }

        var contentTypeExtensionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["application/pdf"] = ".pdf",
            ["application/vnd.openxmlformats-officedocument.wordprocessingml.document"] = ".docx"
        };

        var extension = Path.GetExtension(file.FileName);
        var isSupported = _fileValidationOptions.SupportedContentTypes.Any(ct =>
            string.Equals(file.ContentType, ct, StringComparison.OrdinalIgnoreCase) ||
            (contentTypeExtensionMap.TryGetValue(ct, out var ext) &&
             string.Equals(extension, ext, StringComparison.OrdinalIgnoreCase)));

        if (!isSupported)
        {
            var supportedExtensions = _fileValidationOptions.SupportedContentTypes
                .Where(ct => contentTypeExtensionMap.ContainsKey(ct))
                .Select(ct => contentTypeExtensionMap[ct].ToUpperInvariant())
                .ToList();
            var formatsText = string.Join(", ", supportedExtensions);
            ModelState.AddModelError("cvFile", $"Unsupported file format. Supported formats: {formatsText}.");
            return ValidationProblem(ModelState);
        }

        return null;
    }

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

    private static IReadOnlyList<string> NormalizeSkills(IEnumerable<string> skills)
    {
        return skills
            .Where(skill => !string.IsNullOrWhiteSpace(skill))
            .Select(skill => skill.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

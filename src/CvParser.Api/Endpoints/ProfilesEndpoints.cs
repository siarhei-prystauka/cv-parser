using CvParser.Api.Dtos;
using CvParser.Api.Models;
using CvParser.Api.Repositories;
using CvParser.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CvParser.Api.Endpoints;

/// <summary>
/// Defines profile-related API endpoints.
/// </summary>
public static class ProfilesEndpoints
{
    /// <summary>
    /// Maps profile endpoints under the /api/v1 route group.
    /// </summary>
    public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/profiles").WithTags("Profiles");

        group.MapGet("", (IProfileRepository repository) =>
        {
            var profiles = repository.GetAll().Select(ToSummaryDto);
            return Results.Ok(profiles);
        });

        group.MapGet("/{id:guid}", (Guid id, IProfileRepository repository) =>
        {
            var profile = repository.GetById(id);
            if (profile is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(ToDetailDto(profile));
        });

        group.MapPost("/{id:guid}/cv/preview", async (
            Guid id,
            IFormFile cvFile,
            IProfileRepository repository,
            ICvSkillExtractor extractor,
            CancellationToken cancellationToken) =>
        {
            var profile = repository.GetById(id);
            if (profile is null)
            {
                return Results.NotFound();
            }

            var validation = ValidatePdfFile(cvFile);
            if (validation is not null)
            {
                return validation;
            }

            await using var fileStream = cvFile.OpenReadStream();
            var skills = await extractor.ExtractSkillsAsync(fileStream, cvFile.FileName, cancellationToken);
            var response = new CvPreviewResponseDto(cvFile.FileName, skills);

            return Results.Ok(response);
        });

        group.MapPut("/{id:guid}/skills", (
            Guid id,
            [FromBody] UpdateSkillsRequest request,
            IProfileRepository repository) =>
        {
            var validation = ValidateSkills(request);
            if (validation is not null)
            {
                return validation;
            }

            var normalized = NormalizeSkills(request.Skills);
            var updated = repository.UpdateSkills(id, normalized);
            if (updated is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(ToDetailDto(updated));
        });

        return endpoints;
    }

    /// <summary>
    /// Converts a profile into a summary DTO.
    /// </summary>
    private static ProfileSummaryDto ToSummaryDto(EmployeeProfile profile)
    {
        return new ProfileSummaryDto(
            profile.Id,
            profile.FirstName,
            profile.LastName,
            profile.DateOfBirth,
            profile.DepartmentName,
            profile.Skills
        );
    }

    /// <summary>
    /// Converts a profile into a detail DTO.
    /// </summary>
    private static ProfileDetailDto ToDetailDto(EmployeeProfile profile)
    {
        return new ProfileDetailDto(
            profile.Id,
            profile.FirstName,
            profile.LastName,
            profile.DateOfBirth,
            profile.DepartmentName,
            profile.Skills
        );
    }

    /// <summary>
    /// Validates that the provided file is a non-empty PDF.
    /// </summary>
    private static IResult? ValidatePdfFile(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["cvFile"] = ["A PDF file is required."] });
        }

        var extension = Path.GetExtension(file.FileName);
        var isPdf = string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase);

        if (!isPdf)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["cvFile"] = ["Only PDF uploads are supported."] });
        }

        return null;
    }

    /// <summary>
    /// Validates the skills update payload.
    /// </summary>
    private static IResult? ValidateSkills(UpdateSkillsRequest request)
    {
        if (request.Skills is null || request.Skills.Count == 0)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["skills"] = ["At least one skill is required."] });
        }

        if (request.Skills.Any(skill => string.IsNullOrWhiteSpace(skill)))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["skills"] = ["Skills cannot be blank."] });
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

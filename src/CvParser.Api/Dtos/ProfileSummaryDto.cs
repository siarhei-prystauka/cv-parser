namespace CvParser.Api.Dtos;

/// <summary>
/// Represents the profile fields needed for list views.
/// </summary>
public sealed record ProfileSummaryDto(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string DepartmentName,
    IReadOnlyList<string> Skills
);

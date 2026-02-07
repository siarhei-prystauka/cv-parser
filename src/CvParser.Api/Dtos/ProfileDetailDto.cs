namespace CvParser.Api.Dtos;

/// <summary>
/// Represents the complete profile details returned by the API.
/// </summary>
public sealed record ProfileDetailDto(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string DepartmentName,
    IReadOnlyList<string> Skills
);

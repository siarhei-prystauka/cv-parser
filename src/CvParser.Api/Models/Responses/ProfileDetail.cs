namespace CvParser.Api.Models.Responses;

/// <summary>
/// Represents the complete profile details returned by the API.
/// </summary>
public sealed record ProfileDetail(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string DepartmentName,
    IReadOnlyList<string> Skills
);

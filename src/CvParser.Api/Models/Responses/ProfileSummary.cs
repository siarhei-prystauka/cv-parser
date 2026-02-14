namespace CvParser.Api.Models.Responses;

/// <summary>
/// Represents a profile summary for list views.
/// </summary>
public sealed record ProfileSummary(
    Guid Id,
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string DepartmentName,
    IReadOnlyList<string> Skills
);

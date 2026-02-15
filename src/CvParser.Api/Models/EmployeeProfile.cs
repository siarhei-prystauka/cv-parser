namespace CvParser.Api.Models;

/// <summary>
/// Represents a newcomer employee profile.
/// </summary>
public sealed class EmployeeProfile
{
    public Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public required string DepartmentName { get; set; }
    public List<string> Skills { get; set; } = [];
}

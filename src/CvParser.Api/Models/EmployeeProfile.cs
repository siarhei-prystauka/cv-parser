namespace CvParser.Api.Models;

/// <summary>
/// Represents a newcomer profile stored in the system.
/// </summary>
public sealed class EmployeeProfile
{
    /// <summary>
    /// Gets or sets the unique identifier for the profile.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the employee first name.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the employee last name.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Gets or sets the date of birth for the employee.
    /// </summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the department name.
    /// </summary>
    public required string DepartmentName { get; set; }

    /// <summary>
    /// Gets or sets the list of skills associated with the employee.
    /// </summary>
    public List<string> Skills { get; set; } = [];
}

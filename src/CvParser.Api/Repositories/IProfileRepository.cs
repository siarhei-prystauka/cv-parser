using CvParser.Api.Models;

namespace CvParser.Api.Repositories;

/// <summary>
/// Provides read and write access to employee profiles.
/// </summary>
public interface IProfileRepository
{
    /// <summary>
    /// Retrieves all profiles in the repository.
    /// </summary>
    IReadOnlyList<EmployeeProfile> GetAll();

    /// <summary>
    /// Retrieves a profile by its unique identifier.
    /// </summary>
    EmployeeProfile? GetById(Guid id);

    /// <summary>
    /// Updates the skills list for a profile.
    /// </summary>
    EmployeeProfile? UpdateSkills(Guid id, IReadOnlyList<string> skills);
}

using CvParser.Api.Models;
using CvParser.Api.Models.Responses;

namespace CvParser.Api.Converters;

/// <summary>
/// Provides conversion methods for employee profile models.
/// </summary>
public sealed class ProfileConverter : IProfileConverter
{
    /// <summary>
    /// Converts a profile into a summary response model.
    /// </summary>
    public ProfileSummary ToSummary(EmployeeProfile profile)
    {
        return new ProfileSummary(
            profile.Id,
            profile.FirstName,
            profile.LastName,
            profile.DateOfBirth,
            profile.DepartmentName,
            profile.Skills
        );
    }

    /// <summary>
    /// Converts a profile into a detail response model.
    /// </summary>
    public ProfileDetail ToDetail(EmployeeProfile profile)
    {
        return new ProfileDetail(
            profile.Id,
            profile.FirstName,
            profile.LastName,
            profile.DateOfBirth,
            profile.DepartmentName,
            profile.Skills
        );
    }
}

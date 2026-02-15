using CvParser.Api.Models;
using CvParser.Api.Models.Responses;

namespace CvParser.Api.Converters;

/// <summary>
/// Converts between domain models and API response models.
/// </summary>
public sealed class ProfileConverter : IProfileConverter
{
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

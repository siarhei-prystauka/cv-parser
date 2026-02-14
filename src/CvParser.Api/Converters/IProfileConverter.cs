using CvParser.Api.Models;
using CvParser.Api.Models.Responses;

namespace CvParser.Api.Converters;

/// <summary>
/// Defines conversion operations for employee profile models.
/// </summary>
public interface IProfileConverter
{
    /// <summary>
    /// Converts a profile into a summary response model.
    /// </summary>
    ProfileSummary ToSummary(EmployeeProfile profile);

    /// <summary>
    /// Converts a profile into a detail response model.
    /// </summary>
    ProfileDetail ToDetail(EmployeeProfile profile);
}

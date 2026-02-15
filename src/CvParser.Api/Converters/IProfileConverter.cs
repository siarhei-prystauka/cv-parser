using CvParser.Api.Models;
using CvParser.Api.Models.Responses;

namespace CvParser.Api.Converters;

public interface IProfileConverter
{
    ProfileSummary ToSummary(EmployeeProfile profile);
    ProfileDetail ToDetail(EmployeeProfile profile);
}

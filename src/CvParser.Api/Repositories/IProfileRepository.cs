using CvParser.Api.Models;

namespace CvParser.Api.Repositories;

public interface IProfileRepository
{
    IReadOnlyList<EmployeeProfile> GetAll();
    EmployeeProfile? GetById(Guid id);
    EmployeeProfile? UpdateSkills(Guid id, IReadOnlyList<string> skills);
}

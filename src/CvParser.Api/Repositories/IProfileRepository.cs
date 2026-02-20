using CvParser.Api.Models;

namespace CvParser.Api.Repositories;

public interface IProfileRepository
{
    Task<IReadOnlyList<EmployeeProfile>> GetAllAsync();
    Task<EmployeeProfile?> GetByIdAsync(Guid id);
    Task<EmployeeProfile?> UpdateSkillsAsync(Guid id, IReadOnlyList<string> skills);
}

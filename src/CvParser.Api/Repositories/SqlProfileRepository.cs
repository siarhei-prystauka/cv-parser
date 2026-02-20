using CvParser.Api.Data;
using CvParser.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CvParser.Api.Repositories;

/// <summary>
/// EF Core backed profile repository.
/// </summary>
public sealed class SqlProfileRepository(ApplicationDbContext context) : IProfileRepository
{
    public async Task<IReadOnlyList<EmployeeProfile>> GetAllAsync()
        => await context.EmployeeProfiles.ToListAsync();

    public async Task<EmployeeProfile?> GetByIdAsync(Guid id)
        => await context.EmployeeProfiles.FindAsync(id);

    public async Task<EmployeeProfile?> UpdateSkillsAsync(Guid id, IReadOnlyList<string> skills)
    {
        var profile = await context.EmployeeProfiles.FindAsync(id);
        if (profile is null)
        {
            return null;
        }

        profile.Skills = skills.ToList();
        await context.SaveChangesAsync();
        return profile;
    }
}

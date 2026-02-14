using CvParser.Api.Models;

namespace CvParser.Api.Repositories;

/// <summary>
/// Stores profiles in memory for local development.
/// </summary>
public sealed class InMemoryProfileRepository : IProfileRepository
{
    private readonly List<EmployeeProfile> _profiles = BuildSeedProfiles();
    private readonly object _lock = new();

    /// <inheritdoc />
    public IReadOnlyList<EmployeeProfile> GetAll()
    {
        lock (_lock)
        {
            return _profiles.Select(CloneProfile).ToList();
        }
    }

    /// <inheritdoc />
    public EmployeeProfile? GetById(Guid id)
    {
        lock (_lock)
        {
            var profile = _profiles.FirstOrDefault(item => item.Id == id);
            return profile is null ? null : CloneProfile(profile);
        }
    }

    /// <inheritdoc />
    public EmployeeProfile? UpdateSkills(Guid id, IReadOnlyList<string> skills)
    {
        lock (_lock)
        {
            var profile = _profiles.FirstOrDefault(item => item.Id == id);
            if (profile is null)
            {
                return null;
            }

            profile.Skills = skills.ToList();
            return CloneProfile(profile);
        }
    }

    /// <summary>
    /// Builds deterministic seed data for local development.
    /// </summary>
    private static List<EmployeeProfile> BuildSeedProfiles()
    {
        return
        [
            new EmployeeProfile
            {
                Id = Guid.Parse("2b4cc3a4-90d3-4e8b-8c16-8c50f6c5f9f1"),
                FirstName = "Aya",
                LastName = "Mori",
                DateOfBirth = new DateOnly(1998, 4, 12),
                DepartmentName = "Product",
                Skills = ["Product discovery", "Roadmapping"]
            },
            new EmployeeProfile
            {
                Id = Guid.Parse("7e06c2fb-7c26-4d36-9f5a-49f5e7e78010"),
                FirstName = "Mateo",
                LastName = "Silva",
                DateOfBirth = new DateOnly(1996, 11, 3),
                DepartmentName = "Engineering",
                Skills = ["TypeScript", "API design", "React"]
            },
            new EmployeeProfile
            {
                Id = Guid.Parse("63a9d4fb-6a52-4a2c-a0a6-3dfe4b5c7f12"),
                FirstName = "Nia",
                LastName = "Okoye",
                DateOfBirth = new DateOnly(1999, 7, 19),
                DepartmentName = "Data",
                Skills = ["SQL", "Python"]
            },
            new EmployeeProfile
            {
                Id = Guid.Parse("b824a2f3-cb16-4a59-bc80-7c00f18c9f5d"),
                FirstName = "Liam",
                LastName = "Keller",
                DateOfBirth = new DateOnly(1995, 2, 6),
                DepartmentName = "Design",
                Skills = ["UX research", "Figma"]
            },
            new EmployeeProfile
            {
                Id = Guid.Parse("cf0cf40c-90f5-4c33-8a35-9a2c85d4a3ef"),
                FirstName = "Hana",
                LastName = "Sato",
                DateOfBirth = new DateOnly(1997, 9, 24),
                DepartmentName = "Operations",
                Skills = ["Process mapping", "Vendor management"]
            },
            new EmployeeProfile
            {
                Id = Guid.Parse("b2d43e2c-0e6c-42b4-b1b0-8cc663aaf5e6"),
                FirstName = "Noah",
                LastName = "Ivanov",
                DateOfBirth = new DateOnly(1994, 12, 18),
                DepartmentName = "Sales",
                Skills = ["Pipeline management", "Negotiation"]
            },
            new EmployeeProfile
            {
                Id = Guid.Parse("593ea1a1-d5b2-4f59-9c4c-fb9c424f2d23"),
                FirstName = "Priya",
                LastName = "Singh",
                DateOfBirth = new DateOnly(1998, 5, 30),
                DepartmentName = "Marketing",
                Skills = ["Content strategy", "Campaign planning"]
            },
            new EmployeeProfile
            {
                Id = Guid.Parse("d2151d6e-6c0a-4cd1-88de-10c525cc1fc4"),
                FirstName = "Omar",
                LastName = "Zahid",
                DateOfBirth = new DateOnly(1993, 3, 14),
                DepartmentName = "Finance",
                Skills = ["Forecasting", "Budgeting"]
            }
        ];
    }

    /// <summary>
    /// Creates a defensive copy to avoid mutating stored data.
    /// </summary>
    private static EmployeeProfile CloneProfile(EmployeeProfile profile)
    {
        return new EmployeeProfile
        {
            Id = profile.Id,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            DateOfBirth = profile.DateOfBirth,
            DepartmentName = profile.DepartmentName,
            Skills = profile.Skills.ToList()
        };
    }
}

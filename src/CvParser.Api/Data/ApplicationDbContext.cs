using CvParser.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CvParser.Api.Data;

/// <summary>
/// EF Core database context for CV Parser.
/// </summary>
public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();
    public DbSet<ApplicationSetting> ApplicationSettings => Set<ApplicationSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EmployeeProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.DepartmentName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Skills).HasColumnType("text[]");
        });

        modelBuilder.Entity<ApplicationSetting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LlmModel).HasMaxLength(200).IsRequired();
        });
    }
}

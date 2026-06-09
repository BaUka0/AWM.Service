namespace AWM.Service.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using AWM.Service.Domain.University;

/// <summary>
/// Read-only DbContext for university master data (Edu_* tables).
/// Does not generate migrations. Uses EnsureCreated() for local dev.
/// All queries should use AsNoTracking().
/// </summary>
public sealed class UniversityDbContext : DbContext
{
    public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeePosition> EmployeePositions => Set<EmployeePosition>();
    public DbSet<OrgUnit> OrgUnits => Set<OrgUnit>();
    public DbSet<OrgUnitType> OrgUnitTypes => Set<OrgUnitType>();
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<SemesterType> SemesterTypes => Set<SemesterType>();
    public DbSet<Speciality> Specialities => Set<Speciality>();
    public DbSet<SpecialityLevel> SpecialityLevels => Set<SpecialityLevel>();
    public DbSet<Specialization> Specializations => Set<Specialization>();
    public DbSet<SpecialitySpecialization> SpecialitySpecializations => Set<SpecialitySpecialization>();
    public DbSet<SpecializationsOrgUnit> SpecializationsOrgUnits => Set<SpecializationsOrgUnit>();
    public DbSet<StudentStatus> StudentStatuses => Set<StudentStatus>();
    public DbSet<Position> Positions => Set<Position>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UniversityDbContext).Assembly,
            type => type.Namespace?.Contains("University") == true);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}

namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Org.Entities;

/// <summary>
/// Repository for NotificationTemplate.
/// </summary>
public interface INotificationTemplateRepository
{
    /// <summary>
    /// Gets a template by ID.
    /// </summary>
    Task<NotificationTemplate?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a template by event type.
    /// </summary>
    Task<NotificationTemplate?> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all templates.
    /// </summary>
    Task<IReadOnlyList<NotificationTemplate>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(NotificationTemplate template, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotificationTemplate template, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for AcademicProgram (dropdown selections).
/// </summary>
public interface IAcademicProgramRepository
{
    /// <summary>
    /// Gets a program by ID.
    /// </summary>
    Task<AcademicProgram?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets programs by department (for student registration dropdowns).
    /// </summary>
    Task<IReadOnlyList<AcademicProgram>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets programs by degree level (Bachelor programs, Master programs, etc.).
    /// </summary>
    Task<IReadOnlyList<AcademicProgram>> GetByDegreeLevelAsync(int degreeLevelId, CancellationToken cancellationToken = default);

    Task AddAsync(AcademicProgram program, CancellationToken cancellationToken = default);
    Task UpdateAsync(AcademicProgram program, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for DegreeLevel (справочник уровней образования).
/// </summary>
public interface IDegreeLevelRepository
{
    /// <summary>
    /// Gets a degree level by ID.
    /// </summary>
    Task<DegreeLevel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all degree levels (Bachelor, Master, PhD).
    /// </summary>
    Task<IReadOnlyList<DegreeLevel>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(DegreeLevel degreeLevel, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for Role (admin panel user management).
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Gets a role by ID.
    /// </summary>
    Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a role by system name.
    /// </summary>
    Task<Role?> GetBySystemNameAsync(string systemName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles (for admin panel dropdowns).
    /// </summary>
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Role role, CancellationToken cancellationToken = default);
}

/// <summary>
/// Lookup repository for organizational entities (Department, Institute).
/// Read-only for dropdown selections.
/// </summary>
public interface IOrganizationLookupRepository
{
    /// <summary>
    /// Gets all institutes for a university.
    /// </summary>
    Task<IReadOnlyList<Institute>> GetInstitutesByUniversityAsync(int universityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all departments for an institute.
    /// </summary>
    Task<IReadOnlyList<Department>> GetDepartmentsByInstituteAsync(int instituteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all departments for a university (flat list for search).
    /// </summary>
    Task<IReadOnlyList<Department>> GetAllDepartmentsAsync(int universityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a department by ID.
    /// </summary>
    Task<Department?> GetDepartmentByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an institute by ID.
    /// </summary>
    Task<Institute?> GetInstituteByIdAsync(int id, CancellationToken cancellationToken = default);
}

namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.University;

/// <summary>
/// Repository for NotificationTemplate.
/// </summary>
public interface INotificationTemplateRepository
{
    Task<NotificationTemplate?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<NotificationTemplate?> GetByEventTypeAsync(string eventType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationTemplate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(NotificationTemplate template, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotificationTemplate template, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for Speciality (read-only, from University).
/// Replaces IAcademicProgramRepository.
/// </summary>
public interface IAcademicProgramRepository
{
    Task<Speciality?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Speciality>> GetByLevelAsync(int levelId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Speciality>> GetAllAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for SpecialityLevel (read-only, from University).
/// Replaces IDegreeLevelRepository.
/// </summary>
public interface IDegreeLevelRepository
{
    Task<SpecialityLevel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SpecialityLevel>> GetAllAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for RoleAccess (RBAC+).
/// Replaces IRoleRepository.
/// </summary>
public interface IRoleRepository
{
    Task<Auth.Entities.RoleAccess?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Auth.Entities.RoleAccess>> GetAllAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Lookup repository for organizational entities (OrgUnit).
/// Replaces IOrganizationLookupRepository.
/// </summary>
public interface IOrganizationLookupRepository
{
    Task<IReadOnlyList<OrgUnit>> GetAllInstitutesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrgUnit>> GetDepartmentsByInstituteAsync(int instituteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrgUnit>> GetAllDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<OrgUnit?> GetDepartmentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrgUnit>> GetDepartmentsByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<OrgUnit?> GetInstituteByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OrgUnit?> GetInstituteByIdTrackedAsync(int id, CancellationToken cancellationToken = default);
    Task<OrgUnit?> GetDepartmentByIdTrackedAsync(int id, CancellationToken cancellationToken = default);
}

namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;

/// <summary>
/// Repository for AcademicYear aggregate - critical for all system operations.
/// </summary>
public interface IAcademicYearRepository
{
    /// <summary>
    /// Gets an academic year by ID.
    /// </summary>
    Task<AcademicYear?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current academic year for a university.
    /// </summary>
    Task<AcademicYear?> GetCurrentAsync(int universityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an academic year that contains the specified date.
    /// </summary>
    Task<AcademicYear?> GetByDateAsync(int universityId, DateTime date, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all academic years for a university.
    /// </summary>
    Task<IReadOnlyList<AcademicYear>> GetByUniversityAsync(int universityId, CancellationToken cancellationToken = default);

    Task AddAsync(AcademicYear academicYear, CancellationToken cancellationToken = default);
    Task UpdateAsync(AcademicYear academicYear, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for Period - workflow stage time constraints.
/// </summary>
public interface IPeriodRepository
{
    /// <summary>
    /// Gets a period by ID.
    /// </summary>
    Task<Period?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the active period for a specific workflow stage in a department.
    /// </summary>
    Task<Period?> GetActiveByStageAsync(
        int departmentId,
        int academicYearId,
        WorkflowStage stage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all periods for a department in an academic year.
    /// </summary>
    Task<IReadOnlyList<Period>> GetByDepartmentAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all periods for a department in an academic year, with tracking.
    /// Used for updates to avoid tracking conflicts.
    /// </summary>
    Task<IReadOnlyList<Period>> GetTrackedByDepartmentAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets any currently active period for the department and year.
    /// </summary>
    Task<Period?> GetActivePeriodAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a workflow stage is currently open.
    /// </summary>
    Task<bool> IsStageOpenAsync(
        int departmentId,
        int academicYearId,
        WorkflowStage stage,
        CancellationToken cancellationToken = default);

    Task AddAsync(Period period, CancellationToken cancellationToken = default);
    Task UpdateAsync(Period period, CancellationToken cancellationToken = default);
}

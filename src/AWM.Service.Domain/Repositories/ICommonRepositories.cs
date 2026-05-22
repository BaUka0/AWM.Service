namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.University;

/// <summary>
/// Repository for SemesterType (read-only, from University).
/// </summary>
public interface ISemesterTypeRepository
{
    Task<SemesterType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SemesterType>> GetAllAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for Semester (read-only, from University).
/// </summary>
public interface ISemesterRepository
{
    Task<Semester?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Semester?> GetCurrentAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Semester>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Semester>> GetByStudyYearAsync(int studyYear, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for WorkflowStage reference data.
/// </summary>
public interface IWorkflowStageRepository
{
    Task<WorkflowStage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowStage>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(WorkflowStage workflowStage, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for Stage - workflow stage time constraints.
/// </summary>
public interface IStageRepository
{
    Task<Stage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Stage?> GetActiveByStageAsync(
        int orgUnitId,
        int semesterId,
        int workflowStageId,
        int? specialityId = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Stage>> GetByDepartmentAsync(
        int orgUnitId,
        int semesterId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Stage>> GetTrackedByDepartmentAsync(
        int orgUnitId,
        int semesterId,
        CancellationToken cancellationToken = default);
    Task<Stage?> GetActiveStageAsync(
        int orgUnitId,
        int semesterId,
        int? specialityId = null,
        CancellationToken cancellationToken = default);
    Task<bool> IsStageOpenAsync(
        int orgUnitId,
        int semesterId,
        int workflowStageId,
        int? specialityId = null,
        CancellationToken cancellationToken = default);
    Task AddAsync(Stage stage, CancellationToken cancellationToken = default);
    Task UpdateAsync(Stage stage, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for unified StaffAssignments.
/// </summary>
public interface IStaffAssignmentRepository
{
    Task<StaffAssignment?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StaffAssignment>> GetByTargetAsync(string targetEntityType, long targetEntityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StaffAssignment>> GetByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StaffAssignment>> GetByRoleAsync(string targetEntityType, long targetEntityId, CommonDomain.Enums.StaffRoleType roleType, CancellationToken cancellationToken = default);
    Task AddAsync(StaffAssignment assignment, CancellationToken cancellationToken = default);
    Task UpdateAsync(StaffAssignment assignment, CancellationToken cancellationToken = default);
}

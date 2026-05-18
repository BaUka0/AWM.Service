namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.CommonDomain.Entities;

/// <summary>
/// Repository for SemesterType reference data.
/// </summary>
public interface ISemesterTypeRepository
{
    Task<SemesterType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SemesterType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(SemesterType semesterType, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for Semester aggregate.
/// </summary>
public interface ISemesterRepository
{
    Task<Semester?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Semester>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Semester>> GetByStudyYearAsync(int studyYear, CancellationToken cancellationToken = default);
    Task AddAsync(Semester semester, CancellationToken cancellationToken = default);
    Task UpdateAsync(Semester semester, CancellationToken cancellationToken = default);
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
    /// <summary>
    /// Gets a stage by ID.
    /// </summary>
    Task<Stage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the active stage for a specific workflow stage in a department.
    /// </summary>
    Task<Stage?> GetActiveByStageAsync(
        int departmentId,
        int semesterId,
        int workflowStageId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all stages for a department in a semester.
    /// </summary>
    Task<IReadOnlyList<Stage>> GetByDepartmentAsync(
        int departmentId,
        int semesterId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all stages for a department in a semester, with tracking.
    /// Used for updates to avoid tracking conflicts.
    /// </summary>
    Task<IReadOnlyList<Stage>> GetTrackedByDepartmentAsync(
        int departmentId,
        int semesterId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets any currently active stage for the department and semester.
    /// </summary>
    Task<Stage?> GetActiveStageAsync(
        int departmentId,
        int semesterId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a workflow stage is currently open.
    /// </summary>
    Task<bool> IsStageOpenAsync(
        int departmentId,
        int semesterId,
        int workflowStageId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Stage stage, CancellationToken cancellationToken = default);
    Task UpdateAsync(Stage stage, CancellationToken cancellationToken = default);
}

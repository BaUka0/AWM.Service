namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.Wf.Entities;

/// <summary>
/// Repository for WorkType dictionary entity.
/// </summary>
public interface IWorkTypeRepository
{
    /// <summary>
    /// Gets a work type by ID.
    /// </summary>
    Task<WorkType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all work types (for dropdown selections on the frontend).
    /// </summary>
    Task<IReadOnlyList<WorkType>> GetAllAsync(CancellationToken cancellationToken = default);
}

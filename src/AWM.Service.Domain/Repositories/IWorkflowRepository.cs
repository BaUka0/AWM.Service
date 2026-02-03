namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.Wf.Entities;

/// <summary>
/// Repository for workflow configuration - WorkType, State, Transition.
/// </summary>
public interface IWorkflowRepository
{
    #region WorkType

    /// <summary>
    /// Gets a work type by ID with all its states.
    /// </summary>
    Task<WorkType?> GetWorkTypeByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all work types (for dropdown selection).
    /// </summary>
    Task<IReadOnlyList<WorkType>> GetAllWorkTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets work types by degree level (e.g., only Bachelor works).
    /// </summary>
    Task<IReadOnlyList<WorkType>> GetWorkTypesByDegreeLevelAsync(int degreeLevelId, CancellationToken cancellationToken = default);

    #endregion

    #region State

    /// <summary>
    /// Gets a state by ID.
    /// </summary>
    Task<State?> GetStateByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a state by system name (e.g., "Draft", "OnReview").
    /// </summary>
    Task<State?> GetStateBySystemNameAsync(int workTypeId, string systemName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all states for a work type (for state machine configuration).
    /// </summary>
    Task<IReadOnlyList<State>> GetStatesByWorkTypeAsync(int workTypeId, CancellationToken cancellationToken = default);

    #endregion

    #region Transition

    /// <summary>
    /// Gets a transition by ID.
    /// </summary>
    Task<Transition?> GetTransitionByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all allowed transitions from a given state.
    /// </summary>
    Task<IReadOnlyList<Transition>> GetTransitionsFromStateAsync(int fromStateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets complete workflow graph for a work type (all transitions).
    /// </summary>
    Task<IReadOnlyList<Transition>> GetWorkflowGraphAsync(int workTypeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a transition is allowed for a given role.
    /// </summary>
    Task<bool> CanTransitionAsync(int fromStateId, int toStateId, int roleId, CancellationToken cancellationToken = default);

    #endregion
}

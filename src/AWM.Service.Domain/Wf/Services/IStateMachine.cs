namespace AWM.Service.Domain.Wf.Services;

using AWM.Service.Domain.Wf.Entities;

/// <summary>
/// Domain service interface for workflow state machine operations.
/// </summary>
public interface IStateMachine
{
    /// <summary>
    /// Gets available transitions from the current state.
    /// </summary>
    Task<IReadOnlyList<Transition>> GetAvailableTransitionsAsync(int currentStateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available transitions that a specific role can perform.
    /// </summary>
    Task<IReadOnlyList<Transition>> GetAvailableTransitionsForRoleAsync(int currentStateId, int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates if a transition from one state to another is allowed.
    /// </summary>
    Task<bool> CanTransitionAsync(int fromStateId, int toStateId, int? roleId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the state entity by ID.
    /// </summary>
    Task<State?> GetStateAsync(int stateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the state entity by system name.
    /// </summary>
    Task<State?> GetStateByNameAsync(string systemName, int workTypeId, CancellationToken cancellationToken = default);
}

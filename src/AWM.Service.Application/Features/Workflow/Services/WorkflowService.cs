namespace AWM.Service.Application.Features.Workflow.Services;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf.Entities;
using AWM.Service.Domain.Wf.Services;

/// <summary>
/// Application-layer implementation of the IStateMachine domain service.
/// Delegates to WorkflowRepository for transition/state lookups.
/// </summary>
public sealed class WorkflowService : IStateMachine
{
    private readonly IWorkflowRepository _workflowRepository;

    public WorkflowService(IWorkflowRepository workflowRepository)
    {
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
    }

    public async Task<IReadOnlyList<Transition>> GetAvailableTransitionsAsync(
        int currentStateId,
        CancellationToken cancellationToken = default)
    {
        var transitions = await _workflowRepository.GetTransitionsFromStateAsync(currentStateId, cancellationToken);
        return transitions
            .Where(t => !t.IsDeleted)
            .ToList()
            .AsReadOnly();
    }

    public async Task<IReadOnlyList<Transition>> GetAvailableTransitionsForRoleAsync(
        int currentStateId,
        int roleId,
        CancellationToken cancellationToken = default)
    {
        var transitions = await _workflowRepository.GetTransitionsFromStateAsync(currentStateId, cancellationToken);
        return transitions
            .Where(t => !t.IsDeleted && t.CanBePerformedBy(roleId))
            .ToList()
            .AsReadOnly();
    }

    public async Task<bool> CanTransitionAsync(
        int fromStateId,
        int toStateId,
        int? roleId = null,
        CancellationToken cancellationToken = default)
    {
        if (roleId.HasValue)
        {
            return await _workflowRepository.CanTransitionAsync(fromStateId, toStateId, roleId.Value, cancellationToken);
        }

        var transitions = await _workflowRepository.GetTransitionsFromStateAsync(fromStateId, cancellationToken);
        return transitions.Any(t => !t.IsDeleted && t.ToStateId == toStateId);
    }

    public async Task<State?> GetStateAsync(int stateId, CancellationToken cancellationToken = default)
    {
        return await _workflowRepository.GetStateByIdAsync(stateId, cancellationToken);
    }

    public async Task<State?> GetStateByNameAsync(string systemName, int workTypeId, CancellationToken cancellationToken = default)
    {
        return await _workflowRepository.GetStateBySystemNameAsync(workTypeId, systemName, cancellationToken);
    }
}

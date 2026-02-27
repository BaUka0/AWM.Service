namespace AWM.Service.Infrastructure.Persistence.Repositories.Workflow;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for workflow configuration.
/// </summary>
public sealed class WorkflowRepository : IWorkflowRepository
{
    private readonly ApplicationDbContext _context;

    public WorkflowRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    #region WorkType

    /// <inheritdoc />
    public async Task<WorkType?> GetWorkTypeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.WorkTypes
            .Include(w => w.States.Where(s => !s.IsDeleted))
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkType>> GetAllWorkTypesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.WorkTypes
            .AsNoTracking()
            .OrderBy(w => w.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkType>> GetWorkTypesByDegreeLevelAsync(int degreeLevelId, CancellationToken cancellationToken = default)
    {
        return await _context.WorkTypes
            .AsNoTracking()
            .Where(w => w.DegreeLevelId == degreeLevelId)
            .OrderBy(w => w.Name)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region State

    /// <inheritdoc />
    public async Task<State?> GetStateByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.States
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<State?> GetStateBySystemNameAsync(int workTypeId, string systemName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(systemName))
            return null;

        return await _context.States
            .Where(s => s.WorkTypeId == workTypeId && s.SystemName == systemName)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<State>> GetStatesByWorkTypeAsync(int workTypeId, CancellationToken cancellationToken = default)
    {
        return await _context.States
            .AsNoTracking()
            .Where(s => s.WorkTypeId == workTypeId)
            .OrderBy(s => s.Id)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Transition

    /// <inheritdoc />
    public async Task<Transition?> GetTransitionByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Transitions
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Transition>> GetTransitionsFromStateAsync(int fromStateId, CancellationToken cancellationToken = default)
    {
        return await _context.Transitions
            .AsNoTracking()
            .Where(t => t.FromStateId == fromStateId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Transition>> GetWorkflowGraphAsync(int workTypeId, CancellationToken cancellationToken = default)
    {
        // Get all state IDs for this work type
        var stateIds = await _context.States
            .Where(s => s.WorkTypeId == workTypeId)
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        // Get all transitions involving these states
        return await _context.Transitions
            .AsNoTracking()
            .Where(t => stateIds.Contains(t.FromStateId))
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> CanTransitionAsync(int fromStateId, int toStateId, int roleId, CancellationToken cancellationToken = default)
    {
        var transition = await _context.Transitions
            .Where(t => t.FromStateId == fromStateId &&
                        t.ToStateId == toStateId &&
                        !t.IsAutomatic &&
                        (t.AllowedRoleId == null || t.AllowedRoleId == roleId))
            .FirstOrDefaultAsync(cancellationToken);

        return transition != null;
    }

    #endregion
}

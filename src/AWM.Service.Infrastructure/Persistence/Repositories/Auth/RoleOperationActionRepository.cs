namespace AWM.Service.Infrastructure.Persistence.Repositories.Auth;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for RoleOperationAction.
/// </summary>
public sealed class RoleOperationActionRepository : IRoleOperationActionRepository
{
    private readonly ApplicationDbContext _context;

    public RoleOperationActionRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<RoleOperationAction?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.RoleOperationActions.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<RoleOperationAction>> GetByRoleAccessIdAsync(int roleAccessId, CancellationToken cancellationToken = default)
    {
        return await _context.RoleOperationActions
            .AsNoTracking()
            .Where(ra => ra.RoleAccessId == roleAccessId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RoleOperationAction>> GetByRoleAccessIdAndOperationIdAsync(int roleAccessId, int roleOperationId, CancellationToken cancellationToken = default)
    {
        return await _context.RoleOperationActions
            .AsNoTracking()
            .Where(ra => ra.RoleAccessId == roleAccessId && ra.RoleOperationId == roleOperationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int roleAccessId, int roleOperationId, int roleActionTypeId, CancellationToken cancellationToken = default)
    {
        return await _context.RoleOperationActions
            .AsNoTracking()
            .AnyAsync(ra => ra.RoleAccessId == roleAccessId
                && ra.RoleOperationId == roleOperationId
                && ra.RoleActionTypeId == roleActionTypeId, cancellationToken);
    }

    public async Task AddAsync(RoleOperationAction roleOperationAction, CancellationToken cancellationToken = default)
    {
        await _context.RoleOperationActions.AddAsync(roleOperationAction, cancellationToken);
    }

    public Task RemoveAsync(RoleOperationAction roleOperationAction, CancellationToken cancellationToken = default)
    {
        _context.RoleOperationActions.Remove(roleOperationAction);
        return Task.CompletedTask;
    }
}

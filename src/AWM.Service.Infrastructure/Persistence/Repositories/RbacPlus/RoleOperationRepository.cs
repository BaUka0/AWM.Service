namespace AWM.Service.Infrastructure.Persistence.Repositories.RbacPlus;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for RoleOperation.
/// </summary>
public sealed class RoleOperationRepository : IRoleOperationRepository
{
    private readonly ApplicationDbContext _context;

    public RoleOperationRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<RoleOperation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.RoleOperations.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<RoleOperation?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return await _context.RoleOperations
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<RoleOperation>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RoleOperations
            .AsNoTracking()
            .OrderBy(o => o.OrderBy)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RoleOperation>> GetByParentIdAsync(int? parentId, CancellationToken cancellationToken = default)
    {
        return await _context.RoleOperations
            .AsNoTracking()
            .Where(o => o.ParentId == parentId)
            .OrderBy(o => o.OrderBy)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RoleOperation>> GetTreeAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RoleOperations
            .AsNoTracking()
            .Include(o => o.Children)
            .Where(o => o.ParentId == null)
            .OrderBy(o => o.OrderBy)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(RoleOperation roleOperation, CancellationToken cancellationToken = default)
    {
        await _context.RoleOperations.AddAsync(roleOperation, cancellationToken);
    }

    public Task UpdateAsync(RoleOperation roleOperation, CancellationToken cancellationToken = default)
    {
        _context.RoleOperations.Update(roleOperation);
        return Task.CompletedTask;
    }
}

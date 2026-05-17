namespace AWM.Service.Infrastructure.Persistence.Repositories.RbacPlus;

using AWM.Service.Domain.Auth.RbacPlus.Entities;
using AWM.Service.Domain.Auth.RbacPlus.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for RoleActionType.
/// </summary>
public sealed class RoleActionTypeRepository : IRoleActionTypeRepository
{
    private readonly ApplicationDbContext _context;

    public RoleActionTypeRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<RoleActionType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.RoleActionTypes.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<RoleActionType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        return await _context.RoleActionTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<IReadOnlyList<RoleActionType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RoleActionTypes
            .AsNoTracking()
            .OrderBy(a => a.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(RoleActionType roleActionType, CancellationToken cancellationToken = default)
    {
        await _context.RoleActionTypes.AddAsync(roleActionType, cancellationToken);
    }
}

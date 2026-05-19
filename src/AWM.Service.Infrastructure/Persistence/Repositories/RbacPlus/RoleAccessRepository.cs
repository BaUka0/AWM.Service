namespace AWM.Service.Infrastructure.Persistence.Repositories.RbacPlus;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Auth.ViewModels;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for RoleAccess.
/// </summary>
public sealed class RoleAccessRepository : IRoleAccessRepository
{
    private readonly ApplicationDbContext _context;

    public RoleAccessRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<RoleAccess?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.RoleAccesses.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<RoleAccess?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        return await _context.RoleAccesses
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Code == code.ToUpperInvariant(), cancellationToken);
    }

    public async Task<IReadOnlyList<RoleAccess>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RoleAccesses
            .AsNoTracking()
            .OrderBy(r => r.Code)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(RoleAccess roleAccess, CancellationToken cancellationToken = default)
    {
        await _context.RoleAccesses.AddAsync(roleAccess, cancellationToken);
    }

    public Task UpdateAsync(RoleAccess roleAccess, CancellationToken cancellationToken = default)
    {
        _context.RoleAccesses.Update(roleAccess);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<RoleAccessMatrix>> GetRoleAccessMatrixAsync(string roleCode, CancellationToken cancellationToken = default)
    {
        return await _context.Set<RoleAccessMatrix>()
            .AsNoTracking()
            .Where(m => m.RoleCode == roleCode)
            .ToListAsync(cancellationToken);
    }
}

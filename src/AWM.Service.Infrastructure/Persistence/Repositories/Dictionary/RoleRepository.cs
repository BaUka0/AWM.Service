namespace AWM.Service.Infrastructure.Persistence.Repositories.Dictionary;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Role.
/// </summary>
public sealed class RoleRepository : IRoleRepository
{
    private readonly ApplicationDbContext _context;

    public RoleRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Role?> GetBySystemNameAsync(string systemName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(systemName))
            return null;

        return await _context.Roles
            .FirstOrDefaultAsync(r => r.SystemName == systemName, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .AsNoTracking()
            .OrderBy(r => r.DisplayName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Role role, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(role);
        await _context.Roles.AddAsync(role, cancellationToken);
    }
}

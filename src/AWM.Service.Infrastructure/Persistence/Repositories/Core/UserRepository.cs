namespace AWM.Service.Infrastructure.Persistence.Repositories.Core;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Repository implementation for User aggregate.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.RoleAssignments)
            .Where(u => !u.IsDeleted)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(login))
            return null;

        return await _context.Users
            .Include(u => u.RoleAssignments)
            .Where(u => !u.IsDeleted && u.IsActive)
            .FirstOrDefaultAsync(u => u.Login == login, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            return null;

        return await _context.Users
            .Include(u => u.RoleAssignments)
            .Where(u => !u.IsDeleted)
            .FirstOrDefaultAsync(u => u.ExternalId == externalId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<User>> GetByUniversityAsync(int universityId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.RoleAssignments)
            .Where(u => !u.IsDeleted && u.UniversityId == universityId)
            .OrderBy(u => u.Login)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        await _context.Users.AddAsync(user, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<User?> GetWithRoleAssignmentsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.RoleAssignments)
                .ThenInclude(ra => ra.Role)
            .Where(u => !u.IsDeleted && u.IsActive)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<User>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        if (ids is null || !ids.Any())
            return new List<User>();

        return await _context.Users
            .AsNoTracking()
            .Where(u => ids.Contains(u.Id) && !u.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}

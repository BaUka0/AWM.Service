namespace AWM.Service.Infrastructure.Persistence.Repositories.Auth;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for UserAccessHistory.
/// </summary>
public sealed class UserAccessHistoryRepository : IUserAccessHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public UserAccessHistoryRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IReadOnlyList<UserAccessHistory>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserAccessHistories
            .AsNoTracking()
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserAccessHistory>> GetByRoleAccessIdAsync(int roleAccessId, CancellationToken cancellationToken = default)
    {
        return await _context.UserAccessHistories
            .AsNoTracking()
            .Where(h => h.RoleAccessId == roleAccessId)
            .OrderByDescending(h => h.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(UserAccessHistory history, CancellationToken cancellationToken = default)
    {
        await _context.UserAccessHistories.AddAsync(history, cancellationToken);
    }
}

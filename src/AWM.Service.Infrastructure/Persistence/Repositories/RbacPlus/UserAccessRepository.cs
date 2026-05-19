namespace AWM.Service.Infrastructure.Persistence.Repositories.RbacPlus;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Auth.ViewModels;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for UserAccess.
/// </summary>
public sealed class UserAccessRepository : IUserAccessRepository
{
    private readonly ApplicationDbContext _context;

    public UserAccessRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<UserAccess?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.UserAccesses.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<UserAccess>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserAccesses
            .AsNoTracking()
            .Where(ua => ua.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserAccess>> GetByRoleAccessIdAsync(int roleAccessId, CancellationToken cancellationToken = default)
    {
        return await _context.UserAccesses
            .AsNoTracking()
            .Where(ua => ua.RoleAccessId == roleAccessId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int userId, int roleAccessId, CancellationToken cancellationToken = default)
    {
        return await _context.UserAccesses
            .AsNoTracking()
            .AnyAsync(ua => ua.UserId == userId && ua.RoleAccessId == roleAccessId, cancellationToken);
    }

    public async Task AddAsync(UserAccess userAccess, CancellationToken cancellationToken = default)
    {
        await _context.UserAccesses.AddAsync(userAccess, cancellationToken);
    }

    public Task RemoveAsync(UserAccess userAccess, CancellationToken cancellationToken = default)
    {
        _context.UserAccesses.Remove(userAccess);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<string>> CheckUserAccessAsync(int userId, string operationName, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserAccessMatrix>()
            .AsNoTracking()
            .Where(m => m.UserId == userId && m.OperationName == operationName)
            .Select(m => m.ActionTypeName)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserAccessMatrix>> GetUserAccessMatrixAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserAccessMatrix>()
            .AsNoTracking()
            .Where(m => m.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReducedUserAccessMatrix>> GetReducedUserAccessMatrixAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<ReducedUserAccessMatrix>()
            .AsNoTracking()
            .Where(m => m.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}

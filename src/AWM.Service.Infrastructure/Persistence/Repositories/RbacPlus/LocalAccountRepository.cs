namespace AWM.Service.Infrastructure.Persistence.Repositories.RbacPlus;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for LocalAccount.
/// </summary>
public sealed class LocalAccountRepository : ILocalAccountRepository
{
    private readonly ApplicationDbContext _context;

    public LocalAccountRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<LocalAccount?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.LocalAccounts
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<LocalAccount?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.LocalAccounts
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
    }

    public async Task<LocalAccount?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return null;

        return await _context.LocalAccounts
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.RefreshToken == refreshToken, cancellationToken);
    }

    public async Task AddAsync(LocalAccount localAccount, CancellationToken cancellationToken = default)
    {
        await _context.LocalAccounts.AddAsync(localAccount, cancellationToken);
    }

    public Task UpdateAsync(LocalAccount localAccount, CancellationToken cancellationToken = default)
    {
        _context.LocalAccounts.Update(localAccount);
        return Task.CompletedTask;
    }
}

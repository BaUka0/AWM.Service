namespace AWM.Service.Infrastructure.Persistence.Repositories.Defense;

using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for PreDefenseAttempt entity.
/// </summary>
public sealed class PreDefenseAttemptRepository : IPreDefenseAttemptRepository
{
    private readonly ApplicationDbContext _context;

    public PreDefenseAttemptRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<PreDefenseAttempt?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.PreDefenseAttempts
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PreDefenseAttempt>> GetByWorkIdAsync(
        long workId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.PreDefenseAttempts
            .AsNoTracking()
            .Where(p => p.WorkId == workId)
            .OrderBy(p => p.PreDefenseNumber)
            .ThenByDescending(p => p.AttemptDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PreDefenseAttempt?> GetLatestByWorkIdAsync(
        long workId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.PreDefenseAttempts
            .Where(p => p.WorkId == workId)
            .OrderByDescending(p => p.PreDefenseNumber)
            .ThenByDescending(p => p.AttemptDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(PreDefenseAttempt attempt, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(attempt);
        await _context.PreDefenseAttempts.AddAsync(attempt, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(PreDefenseAttempt attempt, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(attempt);
        _context.PreDefenseAttempts.Update(attempt);
        return Task.CompletedTask;
    }
}

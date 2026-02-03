namespace AWM.Service.Infrastructure.Persistence.Repositories.Core;

using AWM.Service.Domain.Org.Entities;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for University aggregate.
/// </summary>
public sealed class UniversityRepository : IUniversityRepository
{
    private readonly ApplicationDbContext _context;

    public UniversityRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<University?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Universities
            .Include(u => u.Institutes)
                .ThenInclude(i => i.Departments)
            .Where(u => !u.IsDeleted)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<University?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var normalizedCode = code.ToUpperInvariant();
        
        return await _context.Universities
            .Include(u => u.Institutes)
                .ThenInclude(i => i.Departments)
            .Where(u => !u.IsDeleted)
            .FirstOrDefaultAsync(u => u.Code == normalizedCode, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<University>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Universities
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .OrderBy(u => u.Name)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(University university, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(university);
        await _context.Universities.AddAsync(university, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(University university, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(university);
        _context.Universities.Update(university);
        return Task.CompletedTask;
    }
}

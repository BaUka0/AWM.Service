namespace AWM.Service.Infrastructure.Persistence.Repositories.Dictionary;

using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for DegreeLevel.
/// </summary>
public sealed class DegreeLevelRepository : IDegreeLevelRepository
{
    private readonly ApplicationDbContext _context;

    public DegreeLevelRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<DegreeLevel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.DegreeLevels
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DegreeLevel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DegreeLevels
            .AsNoTracking()
            .OrderBy(d => d.DurationYears)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(DegreeLevel degreeLevel, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(degreeLevel);
        await _context.DegreeLevels.AddAsync(degreeLevel, cancellationToken);
    }
}

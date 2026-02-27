namespace AWM.Service.Infrastructure.Persistence.Repositories.Common;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for AcademicYear.
/// </summary>
public sealed class AcademicYearRepository : IAcademicYearRepository
{
    private readonly ApplicationDbContext _context;

    public AcademicYearRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<AcademicYear?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.AcademicYears
            .Where(a => !a.IsDeleted)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AcademicYear?> GetCurrentAsync(int universityId, CancellationToken cancellationToken = default)
    {
        return await _context.AcademicYears
            .Where(a => !a.IsDeleted && a.UniversityId == universityId && a.IsCurrent)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AcademicYear?> GetByDateAsync(int universityId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await _context.AcademicYears
            .Where(a => !a.IsDeleted &&
                        a.UniversityId == universityId &&
                        a.StartDate <= date &&
                        a.EndDate >= date)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AcademicYear>> GetByUniversityAsync(int universityId, CancellationToken cancellationToken = default)
    {
        return await _context.AcademicYears
            .AsNoTracking()
            .Where(a => !a.IsDeleted && a.UniversityId == universityId)
            .OrderByDescending(a => a.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(academicYear);
        await _context.AcademicYears.AddAsync(academicYear, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(academicYear);
        _context.AcademicYears.Update(academicYear);
        return Task.CompletedTask;
    }
}

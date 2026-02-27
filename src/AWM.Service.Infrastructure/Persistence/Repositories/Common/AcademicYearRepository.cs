namespace AWM.Service.Infrastructure.Persistence.Repositories.Common;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for AcademicYear.
/// </summary>
public sealed class AcademicYearRepository : RepositoryBase<AcademicYear, int>, IAcademicYearRepository
{
    public AcademicYearRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<AcademicYear?> GetCurrentAsync(int universityId, CancellationToken cancellationToken = default)
    {
        return await Context.AcademicYears
            .Where(a => a.UniversityId == universityId && a.IsCurrent)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AcademicYear?> GetByDateAsync(int universityId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await Context.AcademicYears
            .Where(a => a.UniversityId == universityId &&
                        a.StartDate <= date &&
                        a.EndDate >= date)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AcademicYear>> GetByUniversityAsync(int universityId, CancellationToken cancellationToken = default)
    {
        return await Context.AcademicYears
            .AsNoTracking()
            .Where(a => a.UniversityId == universityId)
            .OrderByDescending(a => a.StartDate)
            .ToListAsync(cancellationToken);
    }
}

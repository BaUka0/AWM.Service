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
    public async Task<AcademicYear?> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        return await Context.AcademicYears
            .Where(a => a.IsCurrent)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AcademicYear?> GetByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        return await Context.AcademicYears
            .Where(a => a.StartDate <= date &&
                        a.EndDate >= date)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AcademicYear>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Context.AcademicYears
            .AsNoTracking()
            .OrderByDescending(a => a.StartDate)
            .ToListAsync(cancellationToken);
    }
}

namespace AWM.Service.Infrastructure.Persistence.Repositories.Common;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Period.
/// </summary>
public sealed class PeriodRepository : RepositoryBase<Period, int>, IPeriodRepository
{
    public PeriodRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<Period?> GetActiveByStageAsync(
        int departmentId,
        int academicYearId,
        WorkflowStage stage,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await Context.Periods
            .AsNoTracking()
            .Where(p => p.DepartmentId == departmentId &&
                        p.AcademicYearId == academicYearId &&
                        p.WorkflowStage == stage &&
                        p.IsActive &&
                        p.StartDate <= now &&
                        p.EndDate >= now)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Period?> GetActivePeriodAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await Context.Periods
            .AsNoTracking()
            .Where(p => p.DepartmentId == departmentId &&
                        p.AcademicYearId == academicYearId &&
                        p.IsActive &&
                        p.StartDate <= now &&
                        p.EndDate >= now)
            .OrderByDescending(p => p.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Period>> GetByDepartmentAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Periods
            .AsNoTracking()
            .Where(p => p.DepartmentId == departmentId &&
                        p.AcademicYearId == academicYearId)
            .OrderBy(p => p.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Period>> GetTrackedByDepartmentAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Periods
            .Where(p => p.DepartmentId == departmentId &&
                        p.AcademicYearId == academicYearId)
            .OrderBy(p => p.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsStageOpenAsync(
        int departmentId,
        int academicYearId,
        WorkflowStage stage,
        CancellationToken cancellationToken = default)
    {
        var period = await GetActiveByStageAsync(departmentId, academicYearId, stage, cancellationToken);
        return period != null;
    }
}

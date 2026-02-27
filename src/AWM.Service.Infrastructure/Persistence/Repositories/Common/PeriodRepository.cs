namespace AWM.Service.Infrastructure.Persistence.Repositories.Common;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Period.
/// </summary>
public sealed class PeriodRepository : IPeriodRepository
{
    private readonly ApplicationDbContext _context;

    public PeriodRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Period?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Periods
            .Where(p => !p.IsDeleted)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Period?> GetActiveByStageAsync(
        int departmentId,
        int academicYearId,
        WorkflowStage stage,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Periods
            .Where(p => !p.IsDeleted &&
                        p.DepartmentId == departmentId &&
                        p.AcademicYearId == academicYearId &&
                        p.WorkflowStage == stage &&
                        p.IsActive &&
                        p.StartDate <= now &&
                        p.EndDate >= now)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Period>> GetByDepartmentAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Periods
            .AsNoTracking()
            .Where(p => !p.IsDeleted &&
                        p.DepartmentId == departmentId &&
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

    /// <inheritdoc />
    public async Task AddAsync(Period period, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(period);
        await _context.Periods.AddAsync(period, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(Period period, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(period);
        _context.Periods.Update(period);
        return Task.CompletedTask;
    }
}

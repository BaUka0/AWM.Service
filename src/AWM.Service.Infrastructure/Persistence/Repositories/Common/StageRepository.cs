namespace AWM.Service.Infrastructure.Persistence.Repositories.Common;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Stage (formerly Period).
/// </summary>
public sealed class StageRepository : RepositoryBase<Stage, int>, IStageRepository
{
    public StageRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<Stage?> GetActiveByStageAsync(
        int departmentId,
        int semesterId,
        int workflowStageId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await Context.Stages
            .AsNoTracking()
            .Where(s => s.DepartmentId == departmentId &&
                        s.SemesterId == semesterId &&
                        s.WorkflowStageId == workflowStageId &&
                        s.IsActive &&
                        s.StartDate <= now &&
                        s.EndDate >= now)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Stage?> GetActiveStageAsync(
        int departmentId,
        int semesterId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await Context.Stages
            .AsNoTracking()
            .Where(s => s.DepartmentId == departmentId &&
                        s.SemesterId == semesterId &&
                        s.IsActive &&
                        s.StartDate <= now &&
                        s.EndDate >= now)
            .OrderByDescending(s => s.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Stage>> GetByDepartmentAsync(
        int departmentId,
        int semesterId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Stages
            .AsNoTracking()
            .Where(s => s.DepartmentId == departmentId &&
                        s.SemesterId == semesterId)
            .OrderBy(s => s.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Stage>> GetTrackedByDepartmentAsync(
        int departmentId,
        int semesterId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Stages
            .Where(s => s.DepartmentId == departmentId &&
                        s.SemesterId == semesterId)
            .OrderBy(s => s.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsStageOpenAsync(
        int departmentId,
        int semesterId,
        int workflowStageId,
        CancellationToken cancellationToken = default)
    {
        var stage = await GetActiveByStageAsync(departmentId, semesterId, workflowStageId, cancellationToken);
        return stage != null;
    }
}

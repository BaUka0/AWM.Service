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
        int orgUnitId,
        int semesterId,
        int workflowStageId,
        int? specialityId = null,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // If speciality is specified, try speciality-specific stage first
        if (specialityId.HasValue)
        {
            var specialized = await Context.Stages
                .AsNoTracking()
                .Where(s => s.OrgUnitId == orgUnitId &&
                            s.SpecialityId == specialityId.Value &&
                            s.SemesterId == semesterId &&
                            s.WorkflowStageId == workflowStageId &&
                            s.IsActive &&
                            s.StartDate <= now &&
                            s.EndDate >= now)
                .FirstOrDefaultAsync(cancellationToken);

            if (specialized != null)
                return specialized;
        }

        // Fallback to department-level stage
        return await Context.Stages
            .AsNoTracking()
            .Where(s => s.OrgUnitId == orgUnitId &&
                        s.SpecialityId == null &&
                        s.SemesterId == semesterId &&
                        s.WorkflowStageId == workflowStageId &&
                        s.IsActive &&
                        s.StartDate <= now &&
                        s.EndDate >= now)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Stage?> GetActiveStageAsync(
        int orgUnitId,
        int semesterId,
        int? specialityId = null,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        if (specialityId.HasValue)
        {
            var specialized = await Context.Stages
                .AsNoTracking()
                .Where(s => s.OrgUnitId == orgUnitId &&
                            s.SpecialityId == specialityId.Value &&
                            s.SemesterId == semesterId &&
                            s.IsActive &&
                            s.StartDate <= now &&
                            s.EndDate >= now)
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (specialized != null)
                return specialized;
        }

        return await Context.Stages
            .AsNoTracking()
            .Where(s => s.OrgUnitId == orgUnitId &&
                        s.SpecialityId == null &&
                        s.SemesterId == semesterId &&
                        s.IsActive &&
                        s.StartDate <= now &&
                        s.EndDate >= now)
            .OrderByDescending(s => s.StartDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Stage>> GetByDepartmentAsync(
        int orgUnitId,
        int semesterId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Stages
            .AsNoTracking()
            .Where(s => s.OrgUnitId == orgUnitId &&
                        s.SemesterId == semesterId)
            .OrderBy(s => s.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Stage>> GetTrackedByDepartmentAsync(
        int orgUnitId,
        int semesterId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Stages
            .Where(s => s.OrgUnitId == orgUnitId &&
                        s.SemesterId == semesterId)
            .OrderBy(s => s.StartDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsStageOpenAsync(
        int orgUnitId,
        int semesterId,
        int workflowStageId,
        int? specialityId = null,
        CancellationToken cancellationToken = default)
    {
        var stage = await GetActiveByStageAsync(orgUnitId, semesterId, workflowStageId, specialityId, cancellationToken);
        return stage != null;
    }
}

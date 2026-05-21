namespace AWM.Service.Infrastructure.Persistence.Repositories.Defense;

using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Commission aggregate.
/// </summary>
public sealed class CommissionRepository : RepositoryBase<Commission, int>, ICommissionRepository
{
    public CommissionRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<Commission?> GetByIdWithAssignmentsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Commissions
            .Include(c => c.Assignments.Where(a => a.TargetEntityType == "Commission"))
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Commission>> GetByDepartmentAsync(
        int orgUnitId,
        int semesterId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Commissions
            .AsNoTracking()
            .Include(c => c.Assignments.Where(a => a.TargetEntityType == "Commission"))
            .Where(c => c.OrgUnitId == orgUnitId &&
                        c.SemesterId == semesterId)
            .OrderBy(c => c.CommissionTypeId)
            .ThenBy(c => c.PreDefenseNumber)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Commission>> GetByTypeAsync(
        int orgUnitId,
        int semesterId,
        int commissionTypeId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Commissions
            .AsNoTracking()
            .Include(c => c.Assignments.Where(a => a.TargetEntityType == "Commission"))
            .Where(c => c.OrgUnitId == orgUnitId &&
                        c.SemesterId == semesterId &&
                        c.CommissionTypeId == commissionTypeId)
            .OrderBy(c => c.PreDefenseNumber)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task DeleteAsync(Commission commission, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(commission);
        // Soft delete is handled by the domain entity's Delete method
        Context.Commissions.Update(commission);
        return Task.CompletedTask;
    }
}

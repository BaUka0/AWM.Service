namespace AWM.Service.Infrastructure.Persistence.Repositories.Common;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

public sealed class StaffAssignmentRepository : RepositoryBase<StaffAssignment, long>, IStaffAssignmentRepository
{
    public StaffAssignmentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<StaffAssignment>> GetByTargetAsync(string targetEntityType, long targetEntityId, CancellationToken cancellationToken = default)
    {
        return await Context.StaffAssignments
            .Where(a => a.TargetEntityType == targetEntityType && a.TargetEntityId == targetEntityId && a.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StaffAssignment>> GetByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Context.StaffAssignments
            .Where(a => a.UserId == userId && a.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StaffAssignment>> GetByRoleAsync(string targetEntityType, long targetEntityId, StaffRoleType roleType, CancellationToken cancellationToken = default)
    {
        return await Context.StaffAssignments
            .Where(a => a.TargetEntityType == targetEntityType &&
                        a.TargetEntityId == targetEntityId &&
                        a.RoleType == roleType &&
                        a.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StaffAssignment>> GetByTargetsAndRoleAsync(string targetEntityType, IEnumerable<long> targetEntityIds, StaffRoleType roleType, CancellationToken cancellationToken = default)
    {
        var entityIds = targetEntityIds.Distinct().ToList();
        if (entityIds.Count == 0) return [];

        return await Context.StaffAssignments
            .Where(a => a.TargetEntityType == targetEntityType &&
                        entityIds.Contains(a.TargetEntityId) &&
                        a.RoleType == roleType &&
                        a.IsActive)
            .ToListAsync(cancellationToken);
    }
}

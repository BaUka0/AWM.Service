namespace AWM.Service.Infrastructure.Persistence.Repositories.Defense;

using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Protocol.
/// </summary>
public sealed class ProtocolRepository : RepositoryBase<Protocol, long>, IProtocolRepository
{
    public ProtocolRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<Protocol?> GetByScheduleIdAsync(long scheduleId, CancellationToken cancellationToken = default)
    {
        return await Context.Protocols
            .Where(p => p.ScheduleId == scheduleId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Protocol>> GetByCommissionAsync(int commissionId, CancellationToken cancellationToken = default)
    {
        return await Context.Protocols
            .AsNoTracking()
            .Where(p => p.CommissionId == commissionId)
            .OrderByDescending(p => p.SessionDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Protocol>> GetByDepartmentAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Protocols
            .AsNoTracking()
            .Join(
                Context.Commissions.Where(c => !c.IsDeleted && c.DepartmentId == departmentId && c.AcademicYearId == academicYearId),
                p => p.CommissionId,
                c => c.Id,
                (p, c) => p)
            .OrderByDescending(p => p.SessionDate)
            .ToListAsync(cancellationToken);
    }
}

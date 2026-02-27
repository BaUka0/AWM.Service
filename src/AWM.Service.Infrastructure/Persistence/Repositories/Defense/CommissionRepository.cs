namespace AWM.Service.Infrastructure.Persistence.Repositories.Defense;

using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Defense.Enums;
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
    public async Task<Commission?> GetByIdWithMembersAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Commissions
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Commission>> GetByDepartmentAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        return await Context.Commissions
            .AsNoTracking()
            .Include(c => c.Members)
            .Where(c => c.DepartmentId == departmentId &&
                        c.AcademicYearId == academicYearId)
            .OrderBy(c => c.CommissionType)
            .ThenBy(c => c.PreDefenseNumber)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Commission>> GetByTypeAsync(
        int departmentId,
        int academicYearId,
        CommissionType type,
        CancellationToken cancellationToken = default)
    {
        return await Context.Commissions
            .AsNoTracking()
            .Include(c => c.Members)
            .Where(c => c.DepartmentId == departmentId &&
                        c.AcademicYearId == academicYearId &&
                        c.CommissionType == type)
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

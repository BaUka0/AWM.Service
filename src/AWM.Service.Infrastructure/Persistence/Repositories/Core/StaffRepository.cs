namespace AWM.Service.Infrastructure.Persistence.Repositories.Core;

using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Staff aggregate.
/// </summary>
public sealed class StaffRepository : RepositoryBase<Staff, int>, IStaffRepository
{
    public StaffRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<Staff?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Context.Staff
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Staff>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await Context.Staff
            .AsNoTracking()
            .Where(s => s.DepartmentId == departmentId)
            .OrderBy(s => s.Position)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Staff>> GetSupervisorsWithCapacityAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await Context.Staff
            .AsNoTracking()
            .Where(s => s.DepartmentId == departmentId && s.IsSupervisor && s.MaxStudentsLoad > 0)
            .OrderBy(s => s.Position)
            .ToListAsync(cancellationToken);
    }
}

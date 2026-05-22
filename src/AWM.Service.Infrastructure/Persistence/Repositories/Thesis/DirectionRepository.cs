namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Direction aggregate.
/// </summary>
public sealed class DirectionRepository : IDirectionRepository
{
    private readonly ApplicationDbContext _context;

    public DirectionRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Direction?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Directions
            .Include(d => d.Topics.Where(t => !t.IsDeleted))
            .Where(d => !d.IsDeleted)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Direction>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        var distinctIds = ids.Distinct().ToList();
        return await _context.Directions
            .AsNoTracking()
            .Where(d => distinctIds.Contains(d.Id) && !d.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Direction>> GetByDepartmentAsync(
        int departmentId, 
        int academicYearId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Directions
            .AsNoTracking()
            .Where(d => !d.IsDeleted && 
                        d.OrgUnitId == departmentId && 
                        d.SemesterId == academicYearId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Direction>> GetBySupervisorAsync(
        int userId, 
        int academicYearId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.StaffAssignments
            .AsNoTracking()
            .Where(a => a.UserId == userId &&
                        a.RoleType == StaffRoleType.Supervisor &&
                        a.TargetEntityType == "Direction" &&
                        a.IsActive && !a.IsDeleted)
            .Join(_context.Directions.Where(d => !d.IsDeleted && d.SemesterId == academicYearId),
                a => a.TargetEntityId,
                d => d.Id,
                (a, d) => d)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Direction direction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(direction);
        await _context.Directions.AddAsync(direction, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(Direction direction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(direction);
        _context.Directions.Update(direction);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(Direction direction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(direction);
        _context.Directions.Update(direction);
        return Task.CompletedTask;
    }
}

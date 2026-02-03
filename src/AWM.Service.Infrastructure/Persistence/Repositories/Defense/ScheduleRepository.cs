namespace AWM.Service.Infrastructure.Persistence.Repositories.Defense;

using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Schedule aggregate.
/// </summary>
public sealed class ScheduleRepository : IScheduleRepository
{
    private readonly ApplicationDbContext _context;

    public ScheduleRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Schedule?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Schedules
            .Include(s => s.Grades)
            .Where(s => !s.IsDeleted)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Schedule?> GetByWorkIdAsync(long workId, CancellationToken cancellationToken = default)
    {
        return await _context.Schedules
            .Include(s => s.Grades)
            .Where(s => !s.IsDeleted)
            .FirstOrDefaultAsync(s => s.WorkId == workId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Schedule>> GetByCommissionAsync(
        int commissionId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Schedules
            .AsNoTracking()
            .Include(s => s.Grades)
            .Where(s => !s.IsDeleted && s.CommissionId == commissionId)
            .OrderBy(s => s.DefenseDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Schedule>> GetByDateRangeAsync(
        int departmentId, 
        DateTime from, 
        DateTime to, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Schedules
            .AsNoTracking()
            .Include(s => s.Grades)
            .Join(_context.Commissions,
                  schedule => schedule.CommissionId,
                  commission => commission.Id,
                  (schedule, commission) => new { Schedule = schedule, Commission = commission })
            .Where(x => !x.Schedule.IsDeleted && 
                        !x.Commission.IsDeleted &&
                        x.Commission.DepartmentId == departmentId &&
                        x.Schedule.DefenseDate >= from && 
                        x.Schedule.DefenseDate <= to)
            .OrderBy(x => x.Schedule.DefenseDate)
            .Select(x => x.Schedule)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Schedule schedule, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        await _context.Schedules.AddAsync(schedule, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(Schedule schedule, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        _context.Schedules.Update(schedule);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(Schedule schedule, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        _context.Schedules.Update(schedule);
        return Task.CompletedTask;
    }
}

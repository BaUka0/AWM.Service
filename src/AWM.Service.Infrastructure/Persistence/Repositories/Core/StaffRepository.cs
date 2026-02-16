namespace AWM.Service.Infrastructure.Persistence.Repositories.Core;

using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Staff aggregate.
/// </summary>
public sealed class StaffRepository : IStaffRepository
{
    private readonly ApplicationDbContext _context;

    public StaffRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Staff?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .Where(s => !s.IsDeleted)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Staff?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .Where(s => !s.IsDeleted)
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Staff>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Staff
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.DepartmentId == departmentId)
            .OrderBy(s => s.Position)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Staff>> GetSupervisorsWithCapacityAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        // Get all active staff from department with their current student count
        // WorkParticipant links students to works, not staff directly
        // We need to count via StudentWork's supervisor (from Participant or Topic)
        var staffList = await _context.Staff
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.DepartmentId == departmentId && s.IsSupervisor)
            .ToListAsync(cancellationToken);

        // For now, return all staff - capacity checking would require
        // a more complex query joining StudentWork -> WorkParticipant -> Topic -> Supervisor
        // This is a simplified implementation that can be enhanced later
        return staffList
            .Where(s => s.MaxStudentsLoad > 0)
            .OrderBy(s => s.Position)
            .ToList();
    }

    /// <inheritdoc />
    public async Task AddAsync(Staff staff, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(staff);
        await _context.Staff.AddAsync(staff, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(Staff staff, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(staff);
        _context.Staff.Update(staff);
        return Task.CompletedTask;
    }
}

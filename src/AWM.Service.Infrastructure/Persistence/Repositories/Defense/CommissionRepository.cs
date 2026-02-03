namespace AWM.Service.Infrastructure.Persistence.Repositories.Defense;

using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Defense.Enums;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Commission aggregate.
/// </summary>
public sealed class CommissionRepository : ICommissionRepository
{
    private readonly ApplicationDbContext _context;

    public CommissionRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Commission?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Commissions
            .Where(c => !c.IsDeleted)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Commission?> GetByIdWithMembersAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Commissions
            .Include(c => c.Members)
            .Where(c => !c.IsDeleted)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Commission>> GetByDepartmentAsync(
        int departmentId, 
        int academicYearId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.Commissions
            .AsNoTracking()
            .Include(c => c.Members)
            .Where(c => !c.IsDeleted && 
                        c.DepartmentId == departmentId && 
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
        return await _context.Commissions
            .AsNoTracking()
            .Include(c => c.Members)
            .Where(c => !c.IsDeleted && 
                        c.DepartmentId == departmentId && 
                        c.AcademicYearId == academicYearId &&
                        c.CommissionType == type)
            .OrderBy(c => c.PreDefenseNumber)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Commission commission, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(commission);
        await _context.Commissions.AddAsync(commission, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(Commission commission, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(commission);
        _context.Commissions.Update(commission);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(Commission commission, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(commission);
        // Soft delete is handled by the domain entity's Delete method
        // Just mark as modified
        _context.Commissions.Update(commission);
        return Task.CompletedTask;
    }
}

namespace AWM.Service.Infrastructure.Persistence.Repositories.Defense;

using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Protocol.
/// </summary>
public sealed class ProtocolRepository : IProtocolRepository
{
    private readonly ApplicationDbContext _context;

    public ProtocolRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Protocol?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Protocols
            .Where(p => !p.IsDeleted)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Protocol?> GetByScheduleIdAsync(long scheduleId, CancellationToken cancellationToken = default)
    {
        return await _context.Protocols
            .Where(p => !p.IsDeleted && p.ScheduleId == scheduleId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Protocol>> GetByCommissionAsync(int commissionId, CancellationToken cancellationToken = default)
    {
        return await _context.Protocols
            .AsNoTracking()
            .Where(p => !p.IsDeleted && p.CommissionId == commissionId)
            .OrderByDescending(p => p.SessionDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Protocol>> GetByDepartmentAsync(
        int departmentId,
        int academicYearId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Protocols
            .AsNoTracking()
            .Join(
                _context.Commissions.Where(c => !c.IsDeleted && c.DepartmentId == departmentId && c.AcademicYearId == academicYearId),
                p => p.CommissionId,
                c => c.Id,
                (p, c) => p)
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.SessionDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Protocol protocol, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(protocol);
        await _context.Protocols.AddAsync(protocol, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(Protocol protocol, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(protocol);
        _context.Protocols.Update(protocol);
        return Task.CompletedTask;
    }
}

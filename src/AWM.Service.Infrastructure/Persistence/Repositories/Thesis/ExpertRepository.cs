namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Expert.
/// </summary>
public sealed class ExpertRepository : IExpertRepository
{
    private readonly ApplicationDbContext _context;

    public ExpertRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Expert?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Experts
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Expert>> GetByDepartmentAndTypeAsync(
        int departmentId,
        ExpertiseType expertiseType,
        CancellationToken cancellationToken = default)
    {
        return await _context.Experts
            .AsNoTracking()
            .Where(e => e.IsActive &&
                        e.DepartmentId == departmentId &&
                        e.ExpertiseType == expertiseType)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Expert>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Experts
            .AsNoTracking()
            .Where(e => e.IsActive && e.DepartmentId == departmentId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Expert expert, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expert);
        await _context.Experts.AddAsync(expert, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(Expert expert, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expert);
        _context.Experts.Update(expert);
        return Task.CompletedTask;
    }
}

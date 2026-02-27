namespace AWM.Service.Infrastructure.Persistence.Repositories.Defense;

using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for EvaluationCriteria.
/// </summary>
public sealed class EvaluationCriteriaRepository : IEvaluationCriteriaRepository
{
    private readonly ApplicationDbContext _context;

    public EvaluationCriteriaRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<EvaluationCriteria?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.EvaluationCriteria
            .Where(e => !e.IsDeleted)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EvaluationCriteria>> GetByWorkTypeAsync(
        int workTypeId,
        int? departmentId = null,
        CancellationToken cancellationToken = default)
    {
        // First try to get department-specific criteria
        if (departmentId.HasValue)
        {
            var deptCriteria = await _context.EvaluationCriteria
                .AsNoTracking()
                .Where(e => !e.IsDeleted &&
                            e.WorkTypeId == workTypeId &&
                            e.DepartmentId == departmentId)
                .OrderBy(e => e.CriteriaName)
                .ToListAsync(cancellationToken);

            if (deptCriteria.Count > 0)
                return deptCriteria;
        }

        // Fall back to university-wide criteria
        return await _context.EvaluationCriteria
            .AsNoTracking()
            .Where(e => !e.IsDeleted &&
                        e.WorkTypeId == workTypeId &&
                        e.DepartmentId == null)
            .OrderBy(e => e.CriteriaName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EvaluationCriteria>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.EvaluationCriteria
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .OrderBy(e => e.WorkTypeId)
            .ThenBy(e => e.CriteriaName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(EvaluationCriteria criteria, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        await _context.EvaluationCriteria.AddAsync(criteria, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(EvaluationCriteria criteria, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(criteria);
        _context.EvaluationCriteria.Update(criteria);
        return Task.CompletedTask;
    }
}

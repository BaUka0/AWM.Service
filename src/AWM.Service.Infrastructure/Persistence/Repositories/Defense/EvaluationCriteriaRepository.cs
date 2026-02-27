namespace AWM.Service.Infrastructure.Persistence.Repositories.Defense;

using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for EvaluationCriteria.
/// </summary>
public sealed class EvaluationCriteriaRepository : RepositoryBase<EvaluationCriteria, int>, IEvaluationCriteriaRepository
{
    public EvaluationCriteriaRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EvaluationCriteria>> GetByWorkTypeAsync(
        int workTypeId,
        int? departmentId = null,
        CancellationToken cancellationToken = default)
    {
        // First try to get department-specific criteria
        if (departmentId.HasValue)
        {
            var deptCriteria = await Context.EvaluationCriteria
                .AsNoTracking()
                .Where(e => e.WorkTypeId == workTypeId &&
                            e.DepartmentId == departmentId)
                .OrderBy(e => e.CriteriaName)
                .ToListAsync(cancellationToken);

            if (deptCriteria.Count > 0)
                return deptCriteria;
        }

        // Fall back to university-wide criteria
        return await Context.EvaluationCriteria
            .AsNoTracking()
            .Where(e => e.WorkTypeId == workTypeId &&
                        e.DepartmentId == null)
            .OrderBy(e => e.CriteriaName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EvaluationCriteria>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Context.EvaluationCriteria
            .AsNoTracking()
            .OrderBy(e => e.WorkTypeId)
            .ThenBy(e => e.CriteriaName)
            .ToListAsync(cancellationToken);
    }
}

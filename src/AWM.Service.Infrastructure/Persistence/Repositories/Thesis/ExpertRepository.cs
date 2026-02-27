namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Expert.
/// </summary>
public sealed class ExpertRepository : RepositoryBase<Expert, int>, IExpertRepository
{
    public ExpertRepository(ApplicationDbContext context) : base(context) { }


    /// <inheritdoc />
    public async Task<IReadOnlyList<Expert>> GetByDepartmentAndTypeAsync(
        int departmentId,
        ExpertiseType expertiseType,
        CancellationToken cancellationToken = default)
    {
        return await Context.Experts
            .AsNoTracking()
            .Where(e => e.IsActive &&
                        e.DepartmentId == departmentId &&
                        e.ExpertiseType == expertiseType)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Expert>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
    {
        return await Context.Experts
            .AsNoTracking()
            .Where(e => e.IsActive && e.DepartmentId == departmentId)
            .ToListAsync(cancellationToken);
    }

}

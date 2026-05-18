namespace AWM.Service.Infrastructure.Persistence.Repositories.Common;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for SemesterType.
/// </summary>
public sealed class SemesterTypeRepository : RepositoryBase<SemesterType, int>, ISemesterTypeRepository
{
    public SemesterTypeRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<SemesterType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Context.SemesterTypes
            .AsNoTracking()
            .OrderBy(s => s.OrderBy)
            .ToListAsync(cancellationToken);
    }
}

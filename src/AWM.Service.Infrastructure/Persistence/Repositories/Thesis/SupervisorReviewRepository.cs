namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for SupervisorReview.
/// </summary>
public sealed class SupervisorReviewRepository : RepositoryBase<SupervisorReview, long>, ISupervisorReviewRepository
{
    public SupervisorReviewRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<SupervisorReview?> GetByWorkIdAsync(long workId, CancellationToken cancellationToken = default)
    {
        return await Context.SupervisorReviews
            .Where(r => r.WorkId == workId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SupervisorReview>> GetBySupervisorAsync(int supervisorId, CancellationToken cancellationToken = default)
    {
        return await Context.SupervisorReviews
            .AsNoTracking()
            .Where(r => r.SupervisorId == supervisorId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

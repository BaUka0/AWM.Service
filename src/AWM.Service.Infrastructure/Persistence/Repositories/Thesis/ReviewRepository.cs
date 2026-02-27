namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Review (external reviews).
/// </summary>
public sealed class ReviewRepository : RepositoryBase<Review, long>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Review>> GetByWorkIdAsync(long workId, CancellationToken cancellationToken = default)
    {
        return await Context.Reviews
            .AsNoTracking()
            .Where(r => r.WorkId == workId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Review>> GetByReviewerAsync(int reviewerId, CancellationToken cancellationToken = default)
    {
        return await Context.Reviews
            .AsNoTracking()
            .Where(r => r.ReviewerId == reviewerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

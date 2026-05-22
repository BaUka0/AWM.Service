namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for WorkReview.
/// </summary>
public sealed class WorkReviewRepository : RepositoryBase<WorkReview, long>, IWorkReviewRepository
{
    public WorkReviewRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkReview>> GetByWorkIdAsync(long workId, CancellationToken cancellationToken = default)
    {
        return await Context.WorkReviews
            .AsNoTracking()
            .Where(r => r.WorkId == workId && !r.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<WorkReview?> GetByWorkAndTypeAsync(long workId, ReviewType type, CancellationToken cancellationToken = default)
    {
        return await Context.WorkReviews
            .AsNoTracking()
            .Where(r => r.WorkId == workId && r.Type == type && !r.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WorkReview>> GetByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default)
    {
        return await Context.WorkReviews
            .AsNoTracking()
            .Where(r => r.AuthorUserId == authorUserId && !r.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}

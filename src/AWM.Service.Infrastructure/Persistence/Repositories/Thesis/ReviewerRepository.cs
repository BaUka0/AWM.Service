namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for external reviewers.
/// </summary>
public sealed class ReviewerRepository : RepositoryBase<Reviewer, int>, IReviewerRepository
{
    public ReviewerRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Reviewer>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Reviewers
            .AsNoTracking()
            .Where(r => r.IsActive && !r.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Reviewer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetActiveAsync(cancellationToken);

        return await Context.Reviewers
            .AsNoTracking()
            .Where(r => (r.FullName.Contains(searchTerm) || 
                         (r.Organization != null && r.Organization.Contains(searchTerm))) && 
                        !r.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Reviewer>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var reviewerIds = ids.Distinct().ToList();
        if (reviewerIds.Count == 0)
            return [];

        return await Context.Reviewers
            .AsNoTracking()
            .Where(r => reviewerIds.Contains(r.Id) && !r.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Reviewer?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Context.Reviewers
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.IsActive && !r.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

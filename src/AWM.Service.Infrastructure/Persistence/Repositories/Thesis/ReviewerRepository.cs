namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Reviewer aggregate.
/// </summary>
public sealed class ReviewerRepository : RepositoryBase<Reviewer, int>, IReviewerRepository
{
    public ReviewerRepository(ApplicationDbContext context) : base(context) { }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Reviewer>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Reviewers
            .AsNoTracking()
            .Where(r => r.IsActive)
            .OrderBy(r => r.FullName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Reviewer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetActiveAsync(cancellationToken);

        var term = searchTerm.ToLower();
        return await Context.Reviewers
            .AsNoTracking()
            .Where(r => r.IsActive &&
                        (r.FullName.ToLower().Contains(term) ||
                         (r.Organization != null && r.Organization.ToLower().Contains(term))))
            .OrderBy(r => r.FullName)
            .ToListAsync(cancellationToken);
    }
}

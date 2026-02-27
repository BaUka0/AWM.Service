namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for Reviewer aggregate.
/// </summary>
public sealed class ReviewerRepository : IReviewerRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewerRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Reviewer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Reviewers
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Reviewer>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Reviewers
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
        return await _context.Reviewers
            .AsNoTracking()
            .Where(r => r.IsActive &&
                        (r.FullName.ToLower().Contains(term) ||
                         (r.Organization != null && r.Organization.ToLower().Contains(term))))
            .OrderBy(r => r.FullName)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Reviewer reviewer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reviewer);
        await _context.Reviewers.AddAsync(reviewer, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(Reviewer reviewer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reviewer);
        _context.Reviewers.Update(reviewer);
        return Task.CompletedTask;
    }
}

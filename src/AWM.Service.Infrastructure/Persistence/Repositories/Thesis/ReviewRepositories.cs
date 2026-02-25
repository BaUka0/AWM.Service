namespace AWM.Service.Infrastructure.Persistence.Repositories.Thesis;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository implementation for SupervisorReview.
/// </summary>
public sealed class SupervisorReviewRepository : ISupervisorReviewRepository
{
    private readonly ApplicationDbContext _context;

    public SupervisorReviewRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<SupervisorReview?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.SupervisorReviews
            .Where(r => !r.IsDeleted)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SupervisorReview?> GetByWorkIdAsync(long workId, CancellationToken cancellationToken = default)
    {
        return await _context.SupervisorReviews
            .Where(r => !r.IsDeleted && r.WorkId == workId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SupervisorReview>> GetBySupervisorAsync(int supervisorId, CancellationToken cancellationToken = default)
    {
        return await _context.SupervisorReviews
            .AsNoTracking()
            .Where(r => !r.IsDeleted && r.SupervisorId == supervisorId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(SupervisorReview review, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(review);
        await _context.SupervisorReviews.AddAsync(review, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(SupervisorReview review, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(review);
        _context.SupervisorReviews.Update(review);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Repository implementation for Review (external reviews).
/// </summary>
public sealed class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Review?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .Where(r => !r.IsDeleted)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Review>> GetByWorkIdAsync(long workId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(r => !r.IsDeleted && r.WorkId == workId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Review>> GetByReviewerAsync(int reviewerId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews
            .AsNoTracking()
            .Where(r => !r.IsDeleted && r.ReviewerId == reviewerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(review);
        await _context.Reviews.AddAsync(review, cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateAsync(Review review, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(review);
        _context.Reviews.Update(review);
        return Task.CompletedTask;
    }
}

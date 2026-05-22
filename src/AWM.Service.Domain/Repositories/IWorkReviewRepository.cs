namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// Repository for WorkReview (universal review entity).
/// </summary>
public interface IWorkReviewRepository
{
    /// <summary>
    /// Gets a review by ID.
    /// </summary>
    Task<WorkReview?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all reviews for a specific work.
    /// </summary>
    Task<IReadOnlyList<WorkReview>> GetByWorkIdAsync(long workId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific type of review for a work.
    /// </summary>
    Task<WorkReview?> GetByWorkAndTypeAsync(long workId, ReviewType type, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all reviews by an author.
    /// </summary>
    Task<IReadOnlyList<WorkReview>> GetByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new review.
    /// </summary>
    Task AddAsync(WorkReview review, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing review.
    /// </summary>
    Task UpdateAsync(WorkReview review, CancellationToken cancellationToken = default);
}

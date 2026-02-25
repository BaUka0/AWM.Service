namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.Thesis.Entities;

/// <summary>
/// Repository for SupervisorReview (Отзыв научного руководителя).
/// </summary>
public interface ISupervisorReviewRepository
{
    /// <summary>
    /// Gets a supervisor review by ID.
    /// </summary>
    Task<SupervisorReview?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the supervisor review for a specific work.
    /// </summary>
    Task<SupervisorReview?> GetByWorkIdAsync(long workId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all reviews by a supervisor.
    /// </summary>
    Task<IReadOnlyList<SupervisorReview>> GetBySupervisorAsync(int supervisorId, CancellationToken cancellationToken = default);

    Task AddAsync(SupervisorReview review, CancellationToken cancellationToken = default);
    Task UpdateAsync(SupervisorReview review, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for Review (Внешняя рецензия).
/// </summary>
public interface IReviewRepository
{
    /// <summary>
    /// Gets a review by ID.
    /// </summary>
    Task<Review?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the external reviews for a specific work.
    /// </summary>
    Task<IReadOnlyList<Review>> GetByWorkIdAsync(long workId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all reviews by a reviewer.
    /// </summary>
    Task<IReadOnlyList<Review>> GetByReviewerAsync(int reviewerId, CancellationToken cancellationToken = default);

    Task AddAsync(Review review, CancellationToken cancellationToken = default);
    Task UpdateAsync(Review review, CancellationToken cancellationToken = default);
}

namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.Thesis.Entities;
/// <summary>
/// Repository for external reviewers (база внешних рецензентов).
/// </summary>
public interface IReviewerRepository
{
    /// <summary>
    /// Gets a reviewer by ID.
    /// </summary>
    Task<Reviewer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active reviewers (for dropdown selection).
    /// </summary>
    Task<IReadOnlyList<Reviewer>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches reviewers by name or organization.
    /// </summary>
    Task<IReadOnlyList<Reviewer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets multiple reviewers by their IDs in a single query (bulk operation to avoid N+1).
    /// </summary>
    Task<IReadOnlyList<Reviewer>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a reviewer by linked system user ID.
    /// </summary>
    Task<Reviewer?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task AddAsync(Reviewer reviewer, CancellationToken cancellationToken = default);
    Task UpdateAsync(Reviewer reviewer, CancellationToken cancellationToken = default);
}

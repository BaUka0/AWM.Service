namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;

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

    Task AddAsync(Reviewer reviewer, CancellationToken cancellationToken = default);
    Task UpdateAsync(Reviewer reviewer, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for department experts (Norm Control, Software Check, Anti-Plagiarism).
/// </summary>
public interface IExpertRepository
{
    /// <summary>
    /// Gets an expert by ID.
    /// </summary>
    Task<Expert?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets active experts by department and expertise type.
    /// </summary>
    Task<IReadOnlyList<Expert>> GetByDepartmentAndTypeAsync(
        int departmentId, 
        ExpertiseType expertiseType, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active experts for a department.
    /// </summary>
    Task<IReadOnlyList<Expert>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);

    Task AddAsync(Expert expert, CancellationToken cancellationToken = default);
    Task UpdateAsync(Expert expert, CancellationToken cancellationToken = default);
}

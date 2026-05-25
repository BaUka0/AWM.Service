namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.Thesis.Entities;

/// <summary>
/// Repository interface for Direction aggregate.
/// </summary>
public interface IDirectionRepository
{
    Task<Direction?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Direction>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Direction>> GetByOrgUnitAsync(int orgUnitId, int semesterId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Direction>> GetBySupervisorAsync(int userId, int semesterId, CancellationToken cancellationToken = default);
    Task AddAsync(Direction direction, CancellationToken cancellationToken = default);
    Task UpdateAsync(Direction direction, CancellationToken cancellationToken = default);
    Task DeleteAsync(Direction direction, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Topic aggregate.
/// </summary>
public interface ITopicRepository
{
    Task<Topic?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Topic>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Topic>> GetByOrgUnitAsync(int orgUnitId, int semesterId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Topic>> GetByOrgUnitWithApplicationsAsync(int orgUnitId, int semesterId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Topic>> GetBySupervisorAsync(int userId, int semesterId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Topic>> GetAvailableForSelectionAsync(int orgUnitId, int semesterId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all topic applications by student (для страницы "Мои заявки" студента).
    /// </summary>
    Task<IReadOnlyList<TopicApplication>> GetApplicationsByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets topics for the reconciliation stage with applications loaded.
    /// Includes topics with statuses: Approved, Closed, Reconciled, Inactive, NeedsRevision.
    /// </summary>
    Task<IReadOnlyList<Topic>> GetByOrgUnitForReconciliationAsync(int orgUnitId, int semesterId, CancellationToken cancellationToken = default);

    Task AddAsync(Topic topic, CancellationToken cancellationToken = default);
    Task UpdateAsync(Topic topic, CancellationToken cancellationToken = default);
    Task DeleteAsync(Topic topic, CancellationToken cancellationToken = default);

}

/// <summary>
/// Repository interface for StudentWork aggregate.
/// </summary>
public interface IStudentWorkRepository
{
    Task<StudentWork?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentWork>> GetByIdsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentWork>> GetByIdsWithDetailsAsync(IEnumerable<long> ids, CancellationToken cancellationToken = default);
    Task<StudentWork?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentWork>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentWork>> GetByOrgUnitAsync(int orgUnitId, int semesterId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentWork>> GetByOrgUnitWithParticipantsAndQualityChecksAsync(int orgUnitId, int semesterId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentWork>> GetBySupervisorAsync(int userId, int semesterId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentWork>> GetByStateAsync(int stateId, int orgUnitId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets works by department with pagination (recommended for large datasets).
    /// </summary>
    Task<(IReadOnlyList<StudentWork> Items, int TotalCount)> GetByOrgUnitPagedAsync(
        int orgUnitId,
        int semesterId,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets works by state with pagination.
    /// </summary>
    Task<(IReadOnlyList<StudentWork> Items, int TotalCount)> GetByStatePagedAsync(
        int stateId,
        int orgUnitId,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default);

    Task AddAsync(StudentWork work, CancellationToken cancellationToken = default);
    Task UpdateAsync(StudentWork work, CancellationToken cancellationToken = default);
}

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

/// <summary>
/// Repository for mandatory check types configured for each speciality.
/// </summary>
public interface ISpecialityCheckTypeRepository
{
    Task<IReadOnlyList<SpecialityCheckType>> GetBySpecialityAsync(int specialityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SpecialityCheckType>> GetBySpecialitiesAsync(IEnumerable<int> specialityIds, CancellationToken cancellationToken = default);
    Task AddAsync(SpecialityCheckType specialityCheckType, CancellationToken cancellationToken = default);
    Task DeleteAsync(SpecialityCheckType specialityCheckType, CancellationToken cancellationToken = default);
}

public interface IAttachmentTypeRepository
{
    Task<AttachmentType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AttachmentType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttachmentType>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface ICheckTypeRepository
{
    Task<CheckType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CheckType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CheckType>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface ITopicApplicationRepository
{
    /// <summary>
    /// Gets application by ID.
    /// </summary>
    Task<TopicApplication?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets application by ID with related Topic loaded (for authorization checks).
    /// </summary>
    Task<TopicApplication?> GetByIdWithTopicAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all applications for a specific topic (for supervisor to review).
    /// </summary>
    Task<IReadOnlyList<TopicApplication>> GetByTopicIdAsync(long topicId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all applications for multiple topics in a single query (bulk operation to avoid N+1).
    /// </summary>
    Task<IReadOnlyList<TopicApplication>> GetByTopicIdsAsync(IEnumerable<long> topicIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all applications by a student (for student's "My Applications" page).
    /// </summary>
    Task<IReadOnlyList<TopicApplication>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all applications by student for specific academic year.
    /// </summary>
    Task<IReadOnlyList<TopicApplication>> GetByStudentIdAndYearAsync(int studentId, int semesterId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if student already has an application to the topic.
    /// </summary>
    Task<bool> HasStudentAppliedToTopicAsync(int studentId, long topicId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if student has an accepted application in the given academic year.
    /// </summary>
    Task<bool> HasAcceptedApplicationAsync(int studentId, int semesterId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new application.
    /// </summary>
    Task AddAsync(TopicApplication application, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing application.
    /// </summary>
    Task UpdateAsync(TopicApplication application, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes an application.
    /// </summary>
    Task DeleteAsync(TopicApplication application, CancellationToken cancellationToken = default);
}


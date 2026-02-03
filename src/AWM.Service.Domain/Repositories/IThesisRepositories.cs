namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.Thesis.Entities;

/// <summary>
/// Repository interface for Direction aggregate.
/// </summary>
public interface IDirectionRepository
{
    Task<Direction?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Direction>> GetByDepartmentAsync(int departmentId, int academicYearId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Direction>> GetBySupervisorAsync(int supervisorId, int academicYearId, CancellationToken cancellationToken = default);
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
    Task<IReadOnlyList<Topic>> GetByDepartmentAsync(int departmentId, int academicYearId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Topic>> GetBySupervisorAsync(int supervisorId, int academicYearId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Topic>> GetAvailableForSelectionAsync(int departmentId, int academicYearId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all topic applications by student (для страницы "Мои заявки" студента).
    /// </summary>
    Task<IReadOnlyList<TopicApplication>> GetApplicationsByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
    
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
    Task<StudentWork?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentWork>> GetByStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentWork>> GetByDepartmentAsync(int departmentId, int academicYearId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentWork>> GetByStateAsync(int stateId, int departmentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets works by department with pagination (recommended for large datasets).
    /// </summary>
    Task<(IReadOnlyList<StudentWork> Items, int TotalCount)> GetByDepartmentPagedAsync(
        int departmentId, 
        int academicYearId, 
        int skip = 0, 
        int take = 50, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets works by state with pagination.
    /// </summary>
    Task<(IReadOnlyList<StudentWork> Items, int TotalCount)> GetByStatePagedAsync(
        int stateId, 
        int departmentId, 
        int skip = 0, 
        int take = 50, 
        CancellationToken cancellationToken = default);
    
    Task AddAsync(StudentWork work, CancellationToken cancellationToken = default);
    Task UpdateAsync(StudentWork work, CancellationToken cancellationToken = default);
}


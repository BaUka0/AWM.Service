namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.Org.Entities;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Edu.Entities;

/// <summary>
/// Repository interface for University aggregate.
/// </summary>
public interface IUniversityRepository
{
    Task<University?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<University?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<University>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(University university, CancellationToken cancellationToken = default);
    Task UpdateAsync(University university, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for User aggregate.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<User?> GetByExternalIdAsync(string externalId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByUniversityAsync(int universityId, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Student aggregate.
/// </summary>
public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetByProgramAsync(int programId, CancellationToken cancellationToken = default);
    Task AddAsync(Student student, CancellationToken cancellationToken = default);
    Task UpdateAsync(Student student, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Staff aggregate.
/// </summary>
public interface IStaffRepository
{
    Task<Staff?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Staff?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Staff>> GetByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Staff>> GetSupervisorsWithCapacityAsync(int departmentId, CancellationToken cancellationToken = default);
    Task AddAsync(Staff staff, CancellationToken cancellationToken = default);
    Task UpdateAsync(Staff staff, CancellationToken cancellationToken = default);
}

namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.University;
using System.Collections.Generic;

/// <summary>
/// Repository interface for User (read-only, from University).
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIinAsync(string iin, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Student (read-only, from University).
/// </summary>
public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetBySpecialityAsync(int specialityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetByStatusAsync(int statusId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Employee (read-only, from University).
/// </summary>
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Employee?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetAdvisorsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetByOrgUnitAsync(int orgUnitId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetAllWithPositionsAsync(CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdWithPositionsAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);
}

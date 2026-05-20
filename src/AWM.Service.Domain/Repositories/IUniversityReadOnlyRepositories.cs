namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.University;

/// <summary>
/// Read-only repository interfaces for university master data.
/// </summary>

public interface IUserReadOnlyRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIinAsync(string iin, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IStudentReadOnlyRepository
{
    Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Student?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetBySpecialityAsync(int specialityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetByStatusAsync(int statusId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetByYearAsync(int year, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IEmployeeReadOnlyRepository
{
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<Employee?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetAdvisorsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface IOrgUnitReadOnlyRepository
{
    Task<OrgUnit?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrgUnit>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrgUnit>> GetByTypeAsync(int typeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrgUnit>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrgUnit>> GetInstitutesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrgUnit>> GetChildrenAsync(int parentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrgUnit>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all org unit types (reference dictionary).
    /// </summary>
    Task<IReadOnlyList<OrgUnitType>> GetAllTypesAsync(CancellationToken cancellationToken = default);
}

public interface ISemesterReadOnlyRepository
{
    Task<Semester?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Semester?> GetCurrentAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Semester>> GetByStudyYearAsync(int studyYear, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Semester>> GetAllAsync(CancellationToken cancellationToken = default);
}

public interface ISpecialityReadOnlyRepository
{
    Task<Speciality?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Speciality>> GetByLevelAsync(int levelId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Speciality>> GetAllAsync(CancellationToken cancellationToken = default);
}

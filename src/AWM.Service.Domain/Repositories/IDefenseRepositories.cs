namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.Defense.Entities;
/// <summary>
/// Repository interface for Commission aggregate.
/// </summary>
public interface ICommissionRepository
{
    Task<Commission?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Commission?> GetByIdWithMembersAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Commission>> GetByDepartmentAsync(int orgUnitId, int semesterId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Commission>> GetByTypeAsync(int orgUnitId, int semesterId, int commissionTypeId, CancellationToken cancellationToken = default);
    Task AddAsync(Commission commission, CancellationToken cancellationToken = default);
    Task UpdateAsync(Commission commission, CancellationToken cancellationToken = default);
    Task DeleteAsync(Commission commission, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for Schedule aggregate.
/// </summary>
public interface IScheduleRepository
{
    Task<Schedule?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<Schedule?> GetByWorkIdAsync(long workId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Schedule>> GetByCommissionAsync(int commissionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Schedule>> GetByDateRangeAsync(int departmentId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task AddAsync(Schedule schedule, CancellationToken cancellationToken = default);
    Task UpdateAsync(Schedule schedule, CancellationToken cancellationToken = default);
    Task DeleteAsync(Schedule schedule, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for PreDefenseAttempt.
/// </summary>
public interface IPreDefenseAttemptRepository
{
    Task<PreDefenseAttempt?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PreDefenseAttempt>> GetByWorkIdAsync(long workId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PreDefenseAttempt>> GetByWorkIdsAsync(IEnumerable<long> workIds, CancellationToken cancellationToken = default);
    Task<PreDefenseAttempt?> GetLatestByWorkIdAsync(long workId, CancellationToken cancellationToken = default);
    Task AddAsync(PreDefenseAttempt attempt, CancellationToken cancellationToken = default);
    Task UpdateAsync(PreDefenseAttempt attempt, CancellationToken cancellationToken = default);
}

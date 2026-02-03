namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Defense.Enums;

/// <summary>
/// Repository interface for Commission aggregate.
/// </summary>
public interface ICommissionRepository
{
    Task<Commission?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Commission?> GetByIdWithMembersAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Commission>> GetByDepartmentAsync(int departmentId, int academicYearId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Commission>> GetByTypeAsync(int departmentId, int academicYearId, CommissionType type, CancellationToken cancellationToken = default);
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
    Task<PreDefenseAttempt?> GetLatestByWorkIdAsync(long workId, CancellationToken cancellationToken = default);
    Task AddAsync(PreDefenseAttempt attempt, CancellationToken cancellationToken = default);
    Task UpdateAsync(PreDefenseAttempt attempt, CancellationToken cancellationToken = default);
}

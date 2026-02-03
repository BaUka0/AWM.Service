namespace AWM.Service.Domain.Repositories;

using AWM.Service.Domain.Defense.Entities;

/// <summary>
/// Repository for evaluation criteria (динамическая форма оценивания на защите).
/// </summary>
public interface IEvaluationCriteriaRepository
{
    /// <summary>
    /// Gets criteria by ID.
    /// </summary>
    Task<EvaluationCriteria?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets criteria for a specific work type and department.
    /// Returns department-specific criteria if exists, otherwise falls back to university-wide.
    /// </summary>
    Task<IReadOnlyList<EvaluationCriteria>> GetByWorkTypeAsync(
        int workTypeId, 
        int? departmentId = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all criteria ordered by work type.
    /// </summary>
    Task<IReadOnlyList<EvaluationCriteria>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(EvaluationCriteria criteria, CancellationToken cancellationToken = default);
    Task UpdateAsync(EvaluationCriteria criteria, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for defense protocols (реестр протоколов).
/// </summary>
public interface IProtocolRepository
{
    /// <summary>
    /// Gets a protocol by ID.
    /// </summary>
    Task<Protocol?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a protocol by schedule ID.
    /// </summary>
    Task<Protocol?> GetByScheduleIdAsync(long scheduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets protocols for a commission (реестр протоколов комиссии).
    /// </summary>
    Task<IReadOnlyList<Protocol>> GetByCommissionAsync(int commissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all protocols for a department in academic year (для печати реестра).
    /// </summary>
    Task<IReadOnlyList<Protocol>> GetByDepartmentAsync(
        int departmentId, 
        int academicYearId, 
        CancellationToken cancellationToken = default);

    Task AddAsync(Protocol protocol, CancellationToken cancellationToken = default);
    Task UpdateAsync(Protocol protocol, CancellationToken cancellationToken = default);
}

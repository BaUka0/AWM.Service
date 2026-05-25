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
    /// Gets criteria for a specific work type, department and speciality.
    /// Returns speciality-specific criteria if exists, otherwise department-specific, otherwise university-wide.
    /// </summary>
    Task<IReadOnlyList<EvaluationCriteria>> GetByWorkTypeAsync(
        int workTypeId,
        int? orgUnitId = null,
        int? specialityId = null,
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
    /// Gets all protocols for an org unit in a semester (для печати реестра).
    /// </summary>
    Task<IReadOnlyList<Protocol>> GetByOrgUnitAsync(
        int orgUnitId, 
        int semesterId, 
        CancellationToken cancellationToken = default);

    Task AddAsync(Protocol protocol, CancellationToken cancellationToken = default);
    Task UpdateAsync(Protocol protocol, CancellationToken cancellationToken = default);
}

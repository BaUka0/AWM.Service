namespace AWM.Service.Application.Common.Services;

using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;

/// <summary>
/// Application-layer implementation of IStageValidationService.
/// Checks whether specific workflow stages are open for departments.
/// </summary>
public sealed class StageValidationService : IStageValidationService
{
    private readonly IStageRepository _stageRepository;

    public StageValidationService(IStageRepository stageRepository)
    {
        _stageRepository = stageRepository ?? throw new ArgumentNullException(nameof(stageRepository));
    }

    public async Task<bool> IsStageOpenAsync(
        int orgUnitId,
        int semesterId,
        int workflowStageId,
        int? specialityId = null,
        CancellationToken cancellationToken = default)
    {
        return await _stageRepository.IsStageOpenAsync(orgUnitId, semesterId, workflowStageId, specialityId, cancellationToken);
    }

    public async Task<(bool IsAllowed, string? ErrorMessage)> ValidateOperationInStageAsync(
        int orgUnitId,
        int semesterId,
        int workflowStageId,
        int? specialityId = null,
        CancellationToken cancellationToken = default)
    {
        var isOpen = await IsStageOpenAsync(orgUnitId, semesterId, workflowStageId, specialityId, cancellationToken);

        if (!isOpen)
        {
            return (false, "The workflow stage is not currently open for this department/speciality.");
        }

        return (true, null);
    }
}

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
        int departmentId,
        int semesterId,
        int workflowStageId,
        CancellationToken cancellationToken = default)
    {
        return await _stageRepository.IsStageOpenAsync(departmentId, semesterId, workflowStageId, cancellationToken);
    }

    public async Task<(bool IsAllowed, string? ErrorMessage)> ValidateOperationInStageAsync(
        int departmentId,
        int semesterId,
        int workflowStageId,
        CancellationToken cancellationToken = default)
    {
        var isOpen = await IsStageOpenAsync(departmentId, semesterId, workflowStageId, cancellationToken);

        if (!isOpen)
        {
            return (false, "The workflow stage is not currently open for this department.");
        }

        return (true, null);
    }
}

using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Infrastructure.Services;

/// <summary>
/// Service implementation for validating stage-based workflow operations.
/// </summary>
public sealed class StageValidationService : IStageValidationService
{
    private readonly IStageRepository _stageRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="StageValidationService"/> class.
    /// </summary>
    public StageValidationService(IStageRepository stageRepository)
    {
        _stageRepository = stageRepository;
    }

    /// <inheritdoc />
    public async Task<bool> IsStageOpenAsync(
        int orgUnitId,
        int semesterId,
        int workflowStageId,
        int? specialityId = null,
        CancellationToken cancellationToken = default)
    {
        return await _stageRepository.IsStageOpenAsync(orgUnitId, semesterId, workflowStageId, specialityId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(bool IsAllowed, string? ErrorMessage)> ValidateOperationInStageAsync(
        int orgUnitId,
        int semesterId,
        int workflowStageId,
        int? specialityId = null,
        CancellationToken cancellationToken = default)
    {
        var isOpen = await _stageRepository.IsStageOpenAsync(orgUnitId, semesterId, workflowStageId, specialityId, cancellationToken);
        if (isOpen)
        {
            return (true, null);
        }

        return (false, "Операция заблокирована, так как соответствующий этап воркфлоу закрыт или еще не начался.");
    }
}

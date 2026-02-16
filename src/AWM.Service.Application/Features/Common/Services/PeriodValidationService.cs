namespace AWM.Service.Application.Features.Common.Services;

using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;

/// <summary>
/// Application-layer implementation of IPeriodValidationService.
/// Checks whether specific workflow stages are open for departments.
/// </summary>
public sealed class PeriodValidationService : IPeriodValidationService
{
    private readonly IPeriodRepository _periodRepository;

    public PeriodValidationService(IPeriodRepository periodRepository)
    {
        _periodRepository = periodRepository ?? throw new ArgumentNullException(nameof(periodRepository));
    }

    public async Task<bool> IsStageOpenAsync(
        int departmentId,
        int academicYearId,
        WorkflowStage stage,
        CancellationToken cancellationToken = default)
    {
        return await _periodRepository.IsStageOpenAsync(departmentId, academicYearId, stage, cancellationToken);
    }

    public async Task<(bool IsAllowed, string? ErrorMessage)> ValidateOperationInPeriodAsync(
        int departmentId,
        int academicYearId,
        WorkflowStage requiredStage,
        CancellationToken cancellationToken = default)
    {
        var isOpen = await IsStageOpenAsync(departmentId, academicYearId, requiredStage, cancellationToken);

        if (!isOpen)
        {
            return (false, $"The {requiredStage} period is not currently open for this department.");
        }

        return (true, null);
    }
}

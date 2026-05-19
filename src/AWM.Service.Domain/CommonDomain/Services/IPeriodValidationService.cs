namespace AWM.Service.Domain.CommonDomain.Services;

/// <summary>
/// Domain service interface for validating stage-based operations.
/// </summary>
public interface IStageValidationService
{
    /// <summary>
    /// Checks if a specific workflow stage is currently open for a department.
    /// </summary>
    Task<bool> IsStageOpenAsync(int orgUnitId, int semesterId, int workflowStageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that an operation is allowed in the current stage.
    /// Throws or returns error if the stage is closed.
    /// </summary>
    Task<(bool IsAllowed, string? ErrorMessage)> ValidateOperationInStageAsync(
        int orgUnitId,
        int semesterId,
        int workflowStageId,
        CancellationToken cancellationToken = default);
}

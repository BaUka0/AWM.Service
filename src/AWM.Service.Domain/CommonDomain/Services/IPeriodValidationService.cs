namespace AWM.Service.Domain.CommonDomain.Services;

using AWM.Service.Domain.CommonDomain.Enums;

/// <summary>
/// Domain service interface for validating period-based operations.
/// </summary>
public interface IPeriodValidationService
{
    /// <summary>
    /// Checks if a specific workflow stage is currently open for a department.
    /// </summary>
    Task<bool> IsStageOpenAsync(int departmentId, int academicYearId, WorkflowStage stage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that an operation is allowed in the current period.
    /// Throws or returns error if the period is closed.
    /// </summary>
    Task<(bool IsAllowed, string? ErrorMessage)> ValidateOperationInPeriodAsync(
        int departmentId,
        int academicYearId,
        WorkflowStage requiredStage,
        CancellationToken cancellationToken = default);
}

namespace AWM.Service.Application.Features.Workflow.Stages.DTOs;

/// <summary>
/// Data transfer object representing the period for a specific workflow stage.
/// </summary>
public record StagePeriodDto(int WorkflowStageId, DateTime StartDate, DateTime EndDate);

namespace AWM.Service.WebAPI.Common.Contracts.Responses;

public record StagePeriodResponse(int WorkflowStageId, DateTime StartDate, DateTime EndDate);

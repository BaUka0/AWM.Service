namespace AWM.Service.WebAPI.Common.Contracts.Requests;

public record StagePeriodRequest(int WorkflowStageId, DateTime StartDate, DateTime EndDate);

public record SetStagesPeriodsRequest(
    int SemesterId,
    List<StagePeriodRequest> Periods,
    int? OrgUnitId = null,
    int? SpecialityId = null);

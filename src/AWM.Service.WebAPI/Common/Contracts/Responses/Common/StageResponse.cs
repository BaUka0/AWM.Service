namespace AWM.Service.WebAPI.Common.Contracts.Responses.Common;

public record StageResponse
{
    public int Id { get; init; }
    public int OrgUnitId { get; init; }
    public int SemesterId { get; init; }
    public int WorkflowStageId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsActive { get; init; }
    public bool IsCurrentlyOpen { get; init; }
}

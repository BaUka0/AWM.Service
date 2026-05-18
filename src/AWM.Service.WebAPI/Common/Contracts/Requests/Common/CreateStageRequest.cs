namespace AWM.Service.WebAPI.Common.Contracts.Requests.Common;

public record CreateStageRequest
{
    public int DepartmentId { get; init; }
    public int SemesterId { get; init; }
    public int WorkflowStageId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

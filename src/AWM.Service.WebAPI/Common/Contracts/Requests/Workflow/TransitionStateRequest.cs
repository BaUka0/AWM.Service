namespace AWM.Service.WebAPI.Common.Contracts.Requests.Workflow;

public record TransitionStateRequest
{
    public int TargetStateId { get; init; }
    public string? Comment { get; init; }
}

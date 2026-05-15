namespace AWM.Service.WebAPI.Common.Contracts.Requests.Workflow;

/// <summary>
/// Request payload for updating a work type.
/// </summary>
public sealed record UpdateWorkTypeRequest
{
    public string Name { get; init; } = string.Empty;
    public int? DegreeLevelId { get; init; }
}

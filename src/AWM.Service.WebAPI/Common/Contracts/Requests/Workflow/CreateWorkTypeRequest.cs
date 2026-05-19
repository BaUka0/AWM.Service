namespace AWM.Service.WebAPI.Common.Contracts.Requests.Workflow;

/// <summary>
/// Request payload for creating a work type.
/// </summary>
public sealed record CreateWorkTypeRequest
{
    public string Name { get; init; } = string.Empty;
    public int? SpecialityLevelId { get; init; }
}

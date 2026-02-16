namespace AWM.Service.WebAPI.Common.Contracts.Responses.Workflow;

public record TransitionResponse
{
    public int Id { get; init; }
    public int FromStateId { get; init; }
    public string? FromStateName { get; init; }
    public int ToStateId { get; init; }
    public string? ToStateName { get; init; }
    public int? AllowedRoleId { get; init; }
    public bool IsAutomatic { get; init; }
}

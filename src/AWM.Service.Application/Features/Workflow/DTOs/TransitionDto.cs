namespace AWM.Service.Application.Features.Workflow.DTOs;

public sealed record TransitionDto
{
    public int Id { get; init; }
    public int FromStateId { get; init; }
    public string? FromStateName { get; init; }
    public int ToStateId { get; init; }
    public string? ToStateName { get; init; }
    public int? AllowedRoleId { get; init; }
    public bool IsAutomatic { get; init; }
}

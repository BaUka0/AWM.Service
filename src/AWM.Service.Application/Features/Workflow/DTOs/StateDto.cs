namespace AWM.Service.Application.Features.Workflow.DTOs;

public sealed record StateDto
{
    public int Id { get; init; }
    public string Code { get; init; } = null!;
    public string? DisplayName { get; init; }
    public bool IsFinal { get; init; }
    public int WorkTypeId { get; init; }
}

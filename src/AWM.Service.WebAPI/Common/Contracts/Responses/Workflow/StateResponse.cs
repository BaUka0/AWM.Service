namespace AWM.Service.WebAPI.Common.Contracts.Responses.Workflow;

public record StateResponse
{
    public int Id { get; init; }
    public string Code { get; init; } = null!;
    public string? DisplayName { get; init; }
    public bool IsFinal { get; init; }
}

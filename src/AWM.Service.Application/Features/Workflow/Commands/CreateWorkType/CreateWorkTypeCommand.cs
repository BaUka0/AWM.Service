namespace AWM.Service.Application.Features.Workflow.Commands.CreateWorkType;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to create a new work type.
/// </summary>
public sealed record CreateWorkTypeCommand : IRequest<Result<int>>
{
    public string Name { get; init; } = string.Empty;
    public int? SpecialityLevelId { get; init; }
}

namespace AWM.Service.Application.Features.Workflow.Commands.UpdateWorkType;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to update an existing work type.
/// </summary>
public sealed record UpdateWorkTypeCommand : IRequest<Result>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int? SpecialityLevelId { get; init; }
}

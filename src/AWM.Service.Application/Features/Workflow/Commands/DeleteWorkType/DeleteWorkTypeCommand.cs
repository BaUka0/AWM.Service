namespace AWM.Service.Application.Features.Workflow.Commands.DeleteWorkType;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to soft delete a work type.
/// </summary>
public sealed record DeleteWorkTypeCommand : IRequest<Result>
{
    public int Id { get; init; }
}

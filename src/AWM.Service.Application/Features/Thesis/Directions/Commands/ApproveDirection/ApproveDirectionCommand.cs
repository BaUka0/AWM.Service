namespace AWM.Service.Application.Features.Thesis.Directions.Commands.ApproveDirection;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to approve a direction (department action).
/// </summary>
public sealed record ApproveDirectionCommand : IRequest<Result>
{
    /// <summary>
    /// Direction ID to approve.
    /// </summary>
    public long Id { get; init; }

}
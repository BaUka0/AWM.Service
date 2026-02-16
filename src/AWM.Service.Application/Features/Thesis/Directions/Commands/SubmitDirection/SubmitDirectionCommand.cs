namespace AWM.Service.Application.Features.Thesis.Directions.Commands.SubmitDirection;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to submit a direction for department review.
/// </summary>
public sealed record SubmitDirectionCommand : IRequest<Result>
{
    /// <summary>
    /// Direction ID to submit.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// User ID who submits this direction (should be supervisor).
    /// </summary>
    public int SubmittedBy { get; init; }
}
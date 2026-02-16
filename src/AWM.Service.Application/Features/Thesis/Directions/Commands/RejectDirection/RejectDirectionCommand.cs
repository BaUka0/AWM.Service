namespace AWM.Service.Application.Features.Thesis.Directions.Commands.RejectDirection;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to reject a direction (department action).
/// </summary>
public sealed record RejectDirectionCommand : IRequest<Result>
{
    /// <summary>
    /// Direction ID to reject.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// User ID who rejects this direction (department head or authorized person).
    /// </summary>
    public int RejectedBy { get; init; }

    /// <summary>
    /// Rejection reason/comment (optional).
    /// </summary>
    public string? Comment { get; init; }
}
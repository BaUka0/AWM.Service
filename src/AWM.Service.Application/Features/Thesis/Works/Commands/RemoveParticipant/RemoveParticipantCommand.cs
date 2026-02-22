namespace AWM.Service.Application.Features.Thesis.Works.Commands.RemoveParticipant;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to remove a participant from a student work.
/// </summary>
public sealed record RemoveParticipantCommand : IRequest<Result>
{
    /// <summary>
    /// ID of the student work.
    /// </summary>
    public long WorkId { get; init; }

    /// <summary>
    /// ID of the student to remove.
    /// </summary>
    public int StudentId { get; init; }
}

namespace AWM.Service.Application.Features.Thesis.Works.Commands.AddParticipant;

using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to add a participant to a student work (for team works).
/// </summary>
public sealed record AddParticipantCommand : IRequest<Result<long>>
{
    /// <summary>
    /// ID of the student work.
    /// </summary>
    public long WorkId { get; init; }

    /// <summary>
    /// ID of the student to add.
    /// </summary>
    public int StudentId { get; init; }

    /// <summary>
    /// Role of the participant (Leader or Member).
    /// </summary>
    public ParticipantRole Role { get; init; } = ParticipantRole.Member;
}

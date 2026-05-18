namespace AWM.Service.Application.Features.Thesis.Works.Commands.AddParticipant;

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
    /// Role ID of the participant (FK to Thesis.ParticipantRoles). 1 = Leader, 2 = Member.
    /// </summary>
    public int RoleId { get; init; } = 2; // Member = 2
}

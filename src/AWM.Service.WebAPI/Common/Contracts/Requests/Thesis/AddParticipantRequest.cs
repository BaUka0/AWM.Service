namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

using AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// Request contract for adding a participant to a student work.
/// </summary>
public sealed record AddParticipantRequest
{
    /// <summary>
    /// ID of the student to add.
    /// </summary>
    /// <example>100</example>
    public int StudentId { get; init; }

    /// <summary>
    /// Role of the participant (Leader or Member). Defaults to Member.
    /// </summary>
    /// <example>Member</example>
    public ParticipantRole Role { get; init; } = ParticipantRole.Member;
}

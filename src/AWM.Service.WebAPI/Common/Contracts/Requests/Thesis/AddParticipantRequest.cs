namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

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
    /// Role of the participant (1 = Leader, 2 = Member). Defaults to Member (2).
    /// </summary>
    /// <example>2</example>
    public int Role { get; init; } = 2;
}

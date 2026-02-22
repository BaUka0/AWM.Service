namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response contract for work participant data.
/// </summary>
public sealed record WorkParticipantResponse
{
    /// <summary>
    /// Participant record ID.
    /// </summary>
    /// <example>1</example>
    public long Id { get; init; }

    /// <summary>
    /// Student ID.
    /// </summary>
    /// <example>100</example>
    public int StudentId { get; init; }

    /// <summary>
    /// Participant role.
    /// </summary>
    /// <example>Leader</example>
    public string Role { get; init; } = null!;

    /// <summary>
    /// Whether this participant is the team leader.
    /// </summary>
    /// <example>true</example>
    public bool IsLeader { get; init; }

    /// <summary>
    /// Date when the participant joined.
    /// </summary>
    /// <example>2024-09-01T10:00:00Z</example>
    public DateTime JoinedAt { get; init; }
}

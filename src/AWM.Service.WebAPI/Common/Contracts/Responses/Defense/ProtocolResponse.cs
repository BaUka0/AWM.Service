namespace AWM.Service.WebAPI.Common.Contracts.Responses.Defense;

/// <summary>
/// Response contract for a defense session protocol.
/// </summary>
public sealed record ProtocolResponse
{
    /// <summary>Protocol ID.</summary>
    /// <example>1</example>
    public long Id { get; init; }

    /// <summary>Schedule ID the protocol belongs to.</summary>
    /// <example>15</example>
    public long ScheduleId { get; init; }

    /// <summary>Commission ID.</summary>
    /// <example>5</example>
    public int CommissionId { get; init; }

    /// <summary>Date and time of the defense session.</summary>
    /// <example>2024-06-20T09:00:00Z</example>
    public DateTime SessionDate { get; init; }

    /// <summary>Path to the generated protocol document.</summary>
    /// <example>/protocols/2024/protocol_15.pdf</example>
    public string? DocumentPath { get; init; }

    /// <summary>Whether the protocol has been finalized.</summary>
    /// <example>true</example>
    public bool IsFinalized { get; init; }

    /// <summary>User ID who finalized the protocol.</summary>
    /// <example>12</example>
    public int? FinalizedBy { get; init; }

    /// <summary>Date and time the protocol was finalized.</summary>
    public DateTime? FinalizedAt { get; init; }

    /// <summary>Date the protocol was created.</summary>
    public DateTime CreatedAt { get; init; }
}

namespace AWM.Service.Application.Features.Defense.Evaluation.DTOs;

/// <summary>
/// DTO representing a defense session protocol.
/// </summary>
public sealed record ProtocolDto
{
    /// <summary>Protocol ID.</summary>
    public long Id { get; init; }

    /// <summary>Schedule ID the protocol belongs to.</summary>
    public long ScheduleId { get; init; }

    /// <summary>Commission ID.</summary>
    public int CommissionId { get; init; }

    /// <summary>Date and time of the defense session.</summary>
    public DateTime SessionDate { get; init; }

    /// <summary>Path to the generated protocol document.</summary>
    public string? DocumentPath { get; init; }

    /// <summary>Whether the protocol has been finalized.</summary>
    public bool IsFinalized { get; init; }

    /// <summary>User ID who finalized the protocol.</summary>
    public int? FinalizedBy { get; init; }

    /// <summary>Date and time the protocol was finalized.</summary>
    public DateTime? FinalizedAt { get; init; }

    /// <summary>Date the protocol was created.</summary>
    public DateTime CreatedAt { get; init; }
}

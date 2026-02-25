namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

/// <summary>
/// Request contract for generating a defense protocol.
/// </summary>
public sealed record GenerateProtocolRequest
{
    /// <summary>
    /// Schedule ID of the defense session.
    /// </summary>
    /// <example>15</example>
    public long ScheduleId { get; init; }

    /// <summary>
    /// Commission ID for the protocol.
    /// </summary>
    /// <example>5</example>
    public int CommissionId { get; init; }
}

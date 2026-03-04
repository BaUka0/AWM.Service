namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

using System;

/// <summary>
/// Request contract for generating a pre-defense session protocol.
/// </summary>
public sealed record GeneratePreDefenseProtocolRequest
{
    /// <summary>
    /// Commission ID (must be PreDefense type).
    /// </summary>
    /// <example>3</example>
    public int CommissionId { get; init; }

    /// <summary>
    /// Date of the pre-defense session.
    /// </summary>
    /// <example>2024-04-15</example>
    public DateTime SessionDate { get; init; }
}

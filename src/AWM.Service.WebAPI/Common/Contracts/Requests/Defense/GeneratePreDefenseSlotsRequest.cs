namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

using System;

/// <summary>
/// Request contract for generating pre-defense time slots.
/// </summary>
public sealed record GeneratePreDefenseSlotsRequest
{
    /// <summary>
    /// Commission ID (must be PreDefense type).
    /// </summary>
    /// <example>3</example>
    public int CommissionId { get; init; }

    /// <summary>
    /// Date for the pre-defense session.
    /// </summary>
    /// <example>2024-04-15</example>
    public DateTime Date { get; init; }

    /// <summary>
    /// Start time (e.g. 09:00).
    /// </summary>
    /// <example>09:00:00</example>
    public TimeSpan StartTime { get; init; }

    /// <summary>
    /// End time (e.g. 17:00).
    /// </summary>
    /// <example>17:00:00</example>
    public TimeSpan EndTime { get; init; }

    /// <summary>
    /// Duration of each slot in minutes. Default: 30.
    /// </summary>
    /// <example>30</example>
    public int SlotDurationMinutes { get; init; } = 30;

    /// <summary>
    /// Location for the pre-defense session.
    /// </summary>
    /// <example>Ауд. 301-А</example>
    public string? Location { get; init; }
}

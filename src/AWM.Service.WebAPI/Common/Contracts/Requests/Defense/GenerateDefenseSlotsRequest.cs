namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

using System;

public sealed record GenerateDefenseSlotsRequest
{
    public int CommissionId { get; init; }
    public DateTime Date { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public int SlotDurationMinutes { get; init; } = 45;
    public string? Location { get; init; }
}

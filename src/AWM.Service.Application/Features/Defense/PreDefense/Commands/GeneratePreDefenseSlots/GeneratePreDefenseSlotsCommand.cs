namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.GeneratePreDefenseSlots;

using System;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GeneratePreDefenseSlotsCommand : IRequest<Result<int>>
{
    public int CommissionId { get; init; }
    public DateTime Date { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public int SlotDurationMinutes { get; init; } = 30;
    public string? Location { get; init; }
}

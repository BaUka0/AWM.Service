namespace AWM.Service.Application.Features.Defense.Evaluation.Commands.GenerateDefenseSlots;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GenerateDefenseSlotsCommand : IRequest<Result<int>>
{
    public int CommissionId { get; init; }
    public DateTime Date { get; init; }
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
    public int SlotDurationMinutes { get; init; } = 45;
    public string? Location { get; init; }
}

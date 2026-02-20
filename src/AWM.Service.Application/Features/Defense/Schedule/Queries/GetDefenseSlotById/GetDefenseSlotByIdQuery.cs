namespace AWM.Service.Application.Features.Defense.Schedule.Queries.GetDefenseSlotById;

using AWM.Service.Application.Features.Defense.Schedule.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve a single defense slot by its ID with detailed grade information.
/// </summary>
public sealed record GetDefenseSlotByIdQuery : IRequest<Result<DefenseSlotDto>>
{
    /// <summary>
    /// Schedule (slot) ID.
    /// </summary>
    public long SlotId { get; init; }
}

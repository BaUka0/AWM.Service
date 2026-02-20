namespace AWM.Service.Application.Features.Defense.Evaluation.Commands.FinalizeDefense;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command for the secretary to finalize a defense by finalizing the associated protocol.
/// </summary>
public sealed record FinalizeDefenseCommand : IRequest<Result>
{
    /// <summary>
    /// Schedule ID of the defense to finalize.
    /// </summary>
    public long ScheduleId { get; init; }
}

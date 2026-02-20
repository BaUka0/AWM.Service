namespace AWM.Service.Application.Features.Defense.Evaluation.Commands.GenerateProtocol;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to generate a defense session protocol.
/// </summary>
public sealed record GenerateProtocolCommand : IRequest<Result<long>>
{
    /// <summary>
    /// Schedule ID of the defense session.
    /// </summary>
    public long ScheduleId { get; init; }

    /// <summary>
    /// Commission ID for the protocol.
    /// </summary>
    public int CommissionId { get; init; }
}

namespace AWM.Service.Application.Features.Defense.Evaluation.Queries.GetProtocol;

using AWM.Service.Application.Features.Defense.Evaluation.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve a protocol by its ID.
/// </summary>
public sealed record GetProtocolQuery : IRequest<Result<ProtocolDto>>
{
    /// <summary>
    /// Protocol ID.
    /// </summary>
    public long ProtocolId { get; init; }
}

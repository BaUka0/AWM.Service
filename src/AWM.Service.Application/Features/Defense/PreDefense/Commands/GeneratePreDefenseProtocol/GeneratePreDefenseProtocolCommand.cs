namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.GeneratePreDefenseProtocol;

using System;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GeneratePreDefenseProtocolCommand : IRequest<Result<long>>
{
    public int CommissionId { get; init; }
    public DateTime SessionDate { get; init; }
}

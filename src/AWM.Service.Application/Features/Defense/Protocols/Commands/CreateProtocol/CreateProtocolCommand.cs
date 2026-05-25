using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Protocols.Commands.CreateProtocol;

public sealed record CreateProtocolCommand(
    long ScheduleId,
    string? ProtocolNumber = null,
    decimal? FinalScoreNumeric = null,
    string? FinalGradeLetter = null,
    string? Decision = null,
    string? Comments = null) : IRequest<Result<long>>;

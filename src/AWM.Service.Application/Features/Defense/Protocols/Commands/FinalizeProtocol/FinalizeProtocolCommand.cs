using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Protocols.Commands.FinalizeProtocol;

public sealed record FinalizeProtocolCommand(long ProtocolId, bool IsStudentPresent = true) : IRequest<Result>;

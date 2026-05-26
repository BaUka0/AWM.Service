using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Protocols.Queries.GenerateReport;

public sealed record GenerateDefenseReportQuery(long ProtocolId) : IRequest<Result<byte[]>>;

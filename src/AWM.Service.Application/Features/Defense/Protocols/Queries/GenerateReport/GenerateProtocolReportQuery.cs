using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Protocols.Queries.GenerateReport;

public sealed record GenerateProtocolReportQuery(long ProtocolId) : IRequest<Result<byte[]>>;

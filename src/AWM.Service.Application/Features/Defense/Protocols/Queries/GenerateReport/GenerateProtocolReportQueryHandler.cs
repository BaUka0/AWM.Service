using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Defense.Enums;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Defense.Protocols.Queries.GenerateReport;

public sealed class GenerateProtocolReportQueryHandler : IRequestHandler<GenerateProtocolReportQuery, Result<byte[]>>
{
    private readonly IProtocolRepository _protocolRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly ISender _sender;

    public GenerateProtocolReportQueryHandler(
        IProtocolRepository protocolRepository,
        IScheduleRepository scheduleRepository,
        ICommissionRepository commissionRepository,
        ISender sender)
    {
        _protocolRepository = protocolRepository;
        _scheduleRepository = scheduleRepository;
        _commissionRepository = commissionRepository;
        _sender = sender;
    }

    public async Task<Result<byte[]>> Handle(GenerateProtocolReportQuery request, CancellationToken cancellationToken)
    {
        var protocol = await _protocolRepository.GetByIdAsync(request.ProtocolId, cancellationToken);
        if (protocol == null)
        {
            return Result.Failure<byte[]>(new Error("Protocol.NotFound", $"Protocol with ID {request.ProtocolId} not found."));
        }

        var schedule = await _scheduleRepository.GetByIdAsync(protocol.ScheduleId, cancellationToken);
        if (schedule == null)
        {
            return Result.Failure<byte[]>(new Error("Schedule.NotFound", "Associated schedule not found."));
        }

        var commission = await _commissionRepository.GetByIdAsync(schedule.CommissionId, cancellationToken);
        if (commission == null)
        {
            return Result.Failure<byte[]>(new Error("Commission.NotFound", "Associated commission not found."));
        }

        if (commission.CommissionTypeId == (int)CommissionTypes.GAK)
        {
            return await _sender.Send(new GenerateDefenseReportQuery(request.ProtocolId), cancellationToken);
        }
        else
        {
            return await _sender.Send(new GeneratePreDefenseReportQuery(request.ProtocolId), cancellationToken);
        }
    }
}

using AWM.Service.Application.Features.Defense.Schedules.Queries.GetSchedulesByCommission;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Defense.Schedules.Queries.GenerateScheduleReport;

/// <summary>
/// Handler for generating the schedule report PDF.
/// </summary>
public sealed class GenerateScheduleReportQueryHandler
    : IRequestHandler<GenerateScheduleReportQuery, Result<byte[]>>
{
    private readonly ISender _sender;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IPdfReportService _pdfReportService;

    public GenerateScheduleReportQueryHandler(
        ISender sender,
        ICommissionRepository commissionRepository,
        IPdfReportService pdfReportService)
    {
        _sender = sender;
        _commissionRepository = commissionRepository;
        _pdfReportService = pdfReportService;
    }

    public async Task<Result<byte[]>> Handle(
        GenerateScheduleReportQuery request,
        CancellationToken cancellationToken)
    {
        var commission = await _commissionRepository.GetByIdAsync(request.CommissionId, cancellationToken);
        if (commission == null)
        {
            return Result.Failure<byte[]>(new Error("Commission.NotFound", $"Commission with ID {request.CommissionId} not found."));
        }

        var schedulesResult = await _sender.Send(
            new GetSchedulesByCommissionQuery(request.CommissionId),
            cancellationToken);

        if (schedulesResult.IsFailed)
        {
            return Result.Failure<byte[]>(schedulesResult.Error);
        }

        var orderedItems = schedulesResult.Value
            .OrderBy(s => s.DefenseDate)
            .Select(s => new ScheduleReportItem(
                s.Date,
                s.StartTime,
                s.StudentName,
                s.TopicTitle,
                s.Location))
            .ToList();

        var reportData = new ScheduleReportData(
            commission.Name ?? $"Комиссия #{commission.Id}",
            DateTime.UtcNow.ToString("dd.MM.yyyy"),
            orderedItems);

        var pdfBytes = await _pdfReportService.GenerateScheduleReportAsync(reportData);
        return Result.Success(pdfBytes);
    }
}

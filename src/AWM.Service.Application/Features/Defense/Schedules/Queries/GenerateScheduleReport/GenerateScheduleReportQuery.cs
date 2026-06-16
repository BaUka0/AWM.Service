using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Schedules.Queries.GenerateScheduleReport;

/// <summary>
/// Query to generate a PDF report of schedules for a commission.
/// </summary>
public sealed record GenerateScheduleReportQuery(int CommissionId) : IRequest<Result<byte[]>>;

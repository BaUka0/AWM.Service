namespace AWM.Service.Application.Features.Common.Periods.Queries.GetActivePeriod;

using AWM.Service.Application.Features.Common.Periods.DTOs;
using AWM.Service.Domain.CommonDomain.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetActivePeriodQuery : IRequest<Result<PeriodDto?>>
{
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
    public WorkflowStage? WorkflowStage { get; init; }
}

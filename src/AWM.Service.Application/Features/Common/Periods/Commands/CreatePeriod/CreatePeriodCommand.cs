namespace AWM.Service.Application.Features.Common.Periods.Commands.CreatePeriod;

using AWM.Service.Domain.CommonDomain.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record CreatePeriodCommand : IRequest<Result<int>>
{
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
    public WorkflowStage WorkflowStage { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

namespace AWM.Service.Application.Features.Common.Periods.Queries.GetPeriodsByDepartment;

using AWM.Service.Application.Features.Common.Periods.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetPeriodsByDepartmentQuery : IRequest<Result<IReadOnlyList<PeriodDto>>>
{
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
}

namespace AWM.Service.Application.Features.Common.Queries.Periods.GetPeriodsByDepartment;

using AWM.Service.Application.Features.Common.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetPeriodsByDepartmentQuery : IRequest<Result<IReadOnlyList<PeriodDto>>>
{
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
}

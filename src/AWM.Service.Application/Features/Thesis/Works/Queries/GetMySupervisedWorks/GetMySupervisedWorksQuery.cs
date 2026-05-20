namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetMySupervisedWorks;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetMySupervisedWorksQuery : IRequest<Result<(IReadOnlyList<SupervisedWorkDto> Items, int TotalCount)>>
{
    public int? AcademicYearId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

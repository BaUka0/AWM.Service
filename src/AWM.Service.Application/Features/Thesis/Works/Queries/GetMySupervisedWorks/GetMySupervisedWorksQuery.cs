namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetMySupervisedWorks;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetMySupervisedWorksQuery : IRequest<Result<IReadOnlyList<SupervisedWorkDto>>>
{
    public int? AcademicYearId { get; init; }
}

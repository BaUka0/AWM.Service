namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentDefenseStep;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetStudentDefenseStepQuery : IRequest<Result<StudentDefenseStepDto?>>
{
    public long? WorkId { get; init; }
}

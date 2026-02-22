namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetStudentWorkById;

using AWM.Service.Application.Features.Thesis.Works.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get a student work by ID with full details.
/// </summary>
public sealed record GetStudentWorkByIdQuery : IRequest<Result<StudentWorkDetailDto>>
{
    /// <summary>
    /// ID of the student work.
    /// </summary>
    public long WorkId { get; init; }
}

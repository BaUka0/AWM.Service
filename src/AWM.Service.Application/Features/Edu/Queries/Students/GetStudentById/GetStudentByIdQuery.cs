namespace AWM.Service.Application.Features.Edu.Queries.Students.GetStudentById;

using AWM.Service.Application.Features.Edu.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetStudentByIdQuery : IRequest<Result<StudentDto>>
{
    public int StudentId { get; init; }
}

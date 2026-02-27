namespace AWM.Service.Application.Features.Edu.Students.Queries.GetStudentById;

using AWM.Service.Application.Features.Edu.Students.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetStudentByIdQuery : IRequest<Result<StudentDto>>
{
    public int StudentId { get; init; }
}

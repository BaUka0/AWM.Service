namespace AWM.Service.Application.Features.Edu.Students.Queries.GetStudentsByProgram;

using AWM.Service.Application.Features.Edu.Students.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetStudentsByProgramQuery : IRequest<Result<IReadOnlyList<StudentDto>>>
{
    public int ProgramId { get; init; }
}

namespace AWM.Service.Application.Features.Edu.Queries.Students.GetStudentsByProgram;

using AWM.Service.Application.Features.Edu.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetStudentsByProgramQuery : IRequest<Result<IReadOnlyList<StudentDto>>>
{
    public int ProgramId { get; init; }
}

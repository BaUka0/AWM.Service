namespace AWM.Service.Application.Features.Edu.Commands.Students.UpdateStudent;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record UpdateStudentCommand : IRequest<Result>
{
    public int StudentId { get; init; }
    public int? ProgramId { get; init; }
    public string? GroupCode { get; init; }
    public int? CurrentCourse { get; init; }
}

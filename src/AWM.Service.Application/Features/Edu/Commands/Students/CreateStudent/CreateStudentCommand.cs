namespace AWM.Service.Application.Features.Edu.Commands.Students.CreateStudent;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record CreateStudentCommand : IRequest<Result<int>>
{
    public int UserId { get; init; }
    public int ProgramId { get; init; }
    public int AdmissionYear { get; init; }
    public int CurrentCourse { get; init; }
    public string? GroupCode { get; init; }
}

namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetAdmittedStudents;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetAdmittedStudentsQuery : IRequest<Result<IReadOnlyList<AdmittedStudentDto>>>
{
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
}

public sealed record AdmittedStudentDto
{
    public long WorkId { get; init; }
    public int StudentId { get; init; }
    public int UserId { get; init; }
}

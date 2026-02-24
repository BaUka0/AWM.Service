namespace AWM.Service.Application.Features.Thesis.Works.Commands.CreateStudentWork;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to create a new student work after an application is accepted.
/// </summary>
public sealed record CreateStudentWorkCommand : IRequest<Result<long>>
{
    /// <summary>
    /// ID of the topic the work is based on (optional for standalone works).
    /// </summary>
    public long? TopicId { get; init; }

    /// <summary>
    /// Academic year ID.
    /// </summary>
    public int AcademicYearId { get; init; }

    /// <summary>
    /// Department ID.
    /// </summary>
    public int DepartmentId { get; init; }

    /// <summary>
    /// ID of the student who will be the leader/primary participant. 
    /// If not provided, defaults to the current authenticated user.
    /// </summary>
    public int StudentId { get; init; }
}

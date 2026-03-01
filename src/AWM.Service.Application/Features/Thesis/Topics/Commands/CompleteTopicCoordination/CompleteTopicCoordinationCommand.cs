namespace AWM.Service.Application.Features.Thesis.Topics.Commands.CompleteTopicCoordination;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to complete topic coordination for a department.
/// Closes all open topics, finalizes student-topic assignments, and notifies participants.
/// </summary>
public sealed record CompleteTopicCoordinationCommand : IRequest<Result>
{
    /// <summary>
    /// Department ID.
    /// </summary>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Academic year ID.
    /// </summary>
    public int AcademicYearId { get; init; }
}

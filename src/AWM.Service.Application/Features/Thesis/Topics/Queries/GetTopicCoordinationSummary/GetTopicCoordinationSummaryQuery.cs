namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicCoordinationSummary;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get topic coordination summary for a department.
/// Shows overall statistics: total topics, accepted applications, topics without students, etc.
/// </summary>
public sealed record GetTopicCoordinationSummaryQuery : IRequest<Result<TopicCoordinationSummaryDto>>
{
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
}

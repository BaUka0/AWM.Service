namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetAvailableTopics;

using AWM.Service.Application.Features.Thesis.Topics.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve topics available for student selection.
/// Returns approved, open topics with available spots.
/// </summary>
public sealed record GetAvailableTopicsQuery : IRequest<Result<IReadOnlyList<TopicDto>>>
{
    /// <summary>
    /// Department ID to filter topics.
    /// </summary>
    public int DepartmentId { get; init; }

    /// <summary>
    /// Academic year ID to filter topics.
    /// </summary>
    public int AcademicYearId { get; init; }
}
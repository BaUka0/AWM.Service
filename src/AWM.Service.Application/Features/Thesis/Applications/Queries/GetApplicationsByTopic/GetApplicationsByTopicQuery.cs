namespace AWM.Service.Application.Features.Thesis.Applications.Queries.GetApplicationsByTopic;

using AWM.Service.Application.Features.Thesis.Applications.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get all applications for a specific topic.
/// Used by supervisors to review applications.
/// </summary>
public sealed record GetApplicationsByTopicQuery : IRequest<Result<IReadOnlyList<TopicApplicationDto>>>
{
    /// <summary>
    /// ID of the topic.
    /// </summary>
    public long TopicId { get; init; }

    /// <summary>
    /// Optional: Filter by application status.
    /// </summary>
    public int? StatusFilter { get; init; }

    /// <summary>
    /// ID of the requesting user (for authorization check).
    /// </summary>
    public int RequestingUserId { get; init; }
}
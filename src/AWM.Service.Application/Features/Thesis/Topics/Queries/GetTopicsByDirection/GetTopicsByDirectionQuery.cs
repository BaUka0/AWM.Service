namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicsByDirection;

using AWM.Service.Application.Features.Thesis.Topics.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve all topics linked to a specific research direction.
/// </summary>
public sealed record GetTopicsByDirectionQuery : IRequest<Result<IReadOnlyList<TopicDto>>>
{
    /// <summary>
    /// Direction ID to filter topics.
    /// </summary>
    public long DirectionId { get; init; }
}
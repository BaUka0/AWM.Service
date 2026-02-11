namespace AWM.Service.Application.Features.Thesis.Topics.Queries.GetTopicById;

using AWM.Service.Application.Features.Thesis.Topics.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve a specific topic by ID with full details including applications.
/// </summary>
public sealed record GetTopicByIdQuery : IRequest<Result<TopicDetailDto>>
{
    /// <summary>
    /// Topic ID to retrieve.
    /// </summary>
    public long TopicId { get; init; }
}
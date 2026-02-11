namespace AWM.Service.Application.Features.Thesis.Topics.Commands.CloseTopic;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to close a thesis topic.
/// Once closed, the topic no longer accepts new student applications.
/// </summary>
public sealed record CloseTopicCommand : IRequest<Result>
{
    /// <summary>
    /// Topic ID to close.
    /// </summary>
    public long TopicId { get; init; }

    /// <summary>
    /// ID of the user closing the topic (supervisor or department coordinator).
    /// </summary>
    public int ClosedBy { get; init; }
}
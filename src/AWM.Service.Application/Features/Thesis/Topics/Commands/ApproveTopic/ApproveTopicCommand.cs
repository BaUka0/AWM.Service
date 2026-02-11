namespace AWM.Service.Application.Features.Thesis.Topics.Commands.ApproveTopic;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to approve a thesis topic.
/// Department head or authorized person approves topics for student selection.
/// </summary>
public sealed record ApproveTopicCommand : IRequest<Result>
{
    /// <summary>
    /// Topic ID to approve.
    /// </summary>
    public long TopicId { get; init; }

    /// <summary>
    /// ID of the user approving the topic (department head/coordinator).
    /// </summary>
    public int ApprovedBy { get; init; }
}
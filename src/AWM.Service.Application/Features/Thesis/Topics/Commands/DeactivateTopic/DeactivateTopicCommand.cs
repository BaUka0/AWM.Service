namespace AWM.Service.Application.Features.Thesis.Topics.Commands.DeactivateTopic;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to deactivate (soft-delete) a topic that has no accepted students.
/// Used during topic coordination to clean up unused topics.
/// </summary>
public sealed record DeactivateTopicCommand : IRequest<Result>
{
    /// <summary>
    /// Topic ID to deactivate.
    /// </summary>
    public long TopicId { get; init; }
}

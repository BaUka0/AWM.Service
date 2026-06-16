using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.ReviewTopic;

public record ReviewTopicCommand(
    long TopicId,
    bool IsApproved,
    string? Comment = null) : IRequest<Result>;

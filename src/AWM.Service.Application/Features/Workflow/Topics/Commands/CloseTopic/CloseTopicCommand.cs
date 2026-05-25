using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.CloseTopic;

/// <summary>
/// Command to close a topic, preventing new applications.
/// </summary>
public record CloseTopicCommand(long TopicId) : IRequest<Result>;

using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.SubmitTopics;

public record SubmitTopicsCommand(List<long> TopicIds) : IRequest<Result>;

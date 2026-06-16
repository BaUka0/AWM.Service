using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.MarkTopicsInactive;

/// <summary>
/// Command to mark selected topics as inactive (no students applied).
/// Used by the department during the "Согласование тем" stage.
/// </summary>
public record MarkTopicsInactiveCommand(IReadOnlyList<long> TopicIds) : IRequest<Result>;

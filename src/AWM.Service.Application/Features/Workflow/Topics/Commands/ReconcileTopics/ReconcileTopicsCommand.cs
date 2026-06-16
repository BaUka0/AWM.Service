using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.ReconcileTopics;

/// <summary>
/// Command to reconcile (batch final-approve) selected topics.
/// Used by the department during the "Согласование тем" stage.
/// </summary>
public record ReconcileTopicsCommand(IReadOnlyList<long> TopicIds) : IRequest<Result>;

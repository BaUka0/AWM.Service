using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.SendTopicsBackForRevision;

/// <summary>
/// Command to send topics back to supervisors for revision.
/// Used when topics have excess applications and the supervisor needs to resolve them.
/// </summary>
public record SendTopicsBackForRevisionCommand(
    IReadOnlyList<long> TopicIds,
    string Comment) : IRequest<Result>;

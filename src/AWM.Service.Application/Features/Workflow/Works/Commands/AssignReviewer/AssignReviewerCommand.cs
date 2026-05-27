using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.AssignReviewer;

public sealed record AssignReviewerCommand(long WorkId, int ReviewerEntityId) : IRequest<Result>;

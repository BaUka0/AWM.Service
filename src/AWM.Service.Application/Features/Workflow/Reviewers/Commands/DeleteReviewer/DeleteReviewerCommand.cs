using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Reviewers.Commands.DeleteReviewer;

public record DeleteReviewerCommand(int Id) : IRequest<Result>;

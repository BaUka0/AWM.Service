using AWM.Service.Application.Features.Workflow.Reviewers.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Reviewers.Queries.GetAssignedReviewer;

public sealed record GetAssignedReviewerQuery(long WorkId) : IRequest<Result<ReviewerDto?>>;

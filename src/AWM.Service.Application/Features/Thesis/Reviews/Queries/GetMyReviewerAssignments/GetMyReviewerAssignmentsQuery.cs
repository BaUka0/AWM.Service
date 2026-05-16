namespace AWM.Service.Application.Features.Thesis.Reviews.Queries.GetMyReviewerAssignments;

using AWM.Service.Application.Features.Thesis.Reviews.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetMyReviewerAssignmentsQuery : IRequest<Result<IReadOnlyList<ReviewerAssignmentDto>>>;

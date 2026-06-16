using AWM.Service.Application.Features.Workflow.Reviews.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Workflow.Reviews.Queries.GetMyReviewerAssignments;

public record GetMyReviewerAssignmentsQuery() : IRequest<Result<IReadOnlyList<ReviewerAssignmentDto>>>;

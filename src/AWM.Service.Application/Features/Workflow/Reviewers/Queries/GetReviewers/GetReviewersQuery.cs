using AWM.Service.Application.Features.Workflow.Reviewers.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Workflow.Reviewers.Queries.GetReviewers;

public record GetReviewersQuery(string? SearchTerm = null) : IRequest<Result<IReadOnlyList<ReviewerDto>>>;

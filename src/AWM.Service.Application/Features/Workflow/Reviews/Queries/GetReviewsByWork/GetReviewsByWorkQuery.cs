using AWM.Service.Application.Features.Workflow.Reviews.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Workflow.Reviews.Queries.GetReviewsByWork;

public record GetReviewsByWorkQuery(long WorkId) : IRequest<Result<IReadOnlyList<WorkReviewDto>>>;

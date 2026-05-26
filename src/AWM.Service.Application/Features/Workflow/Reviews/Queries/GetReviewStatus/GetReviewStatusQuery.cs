using AWM.Service.Application.Features.Workflow.Reviews.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Workflow.Reviews.Queries.GetReviewStatus;

public record GetReviewStatusQuery(int OrgUnitId, int SemesterId) : IRequest<Result<IReadOnlyList<WorkReviewStatusDto>>>;

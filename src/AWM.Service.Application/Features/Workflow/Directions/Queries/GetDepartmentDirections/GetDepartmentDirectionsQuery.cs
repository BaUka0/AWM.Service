using AWM.Service.Application.Features.Workflow.Directions.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Workflow.Directions.Queries.GetDepartmentDirections;

public record GetDepartmentDirectionsQuery(
    int OrgUnitId,
    int? SemesterId,
    int? StateId) : IRequest<Result<IReadOnlyList<DirectionSummaryDto>>>;

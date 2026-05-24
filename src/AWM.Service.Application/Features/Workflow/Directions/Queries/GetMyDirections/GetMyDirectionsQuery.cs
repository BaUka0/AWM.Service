using AWM.Service.Application.Features.Workflow.Directions.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Workflow.Directions.Queries.GetMyDirections;

public record GetMyDirectionsQuery(
    int? SemesterId) : IRequest<Result<IReadOnlyList<DirectionSummaryDto>>>;

using AWM.Service.Application.Features.Workflow.Directions.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Directions.Queries.GetDirectionById;

/// <summary>
/// Query to get detailed direction by its identifier.
/// </summary>
public record GetDirectionByIdQuery(long Id) : IRequest<Result<DirectionDto>>;

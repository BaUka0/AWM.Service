namespace AWM.Service.Application.Features.Workflow.Queries.GetAllWorkTypes;

using AWM.Service.Application.Features.Workflow.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve all available work types.
/// </summary>
public sealed record GetAllWorkTypesQuery : IRequest<Result<IReadOnlyList<WorkTypeDto>>>;

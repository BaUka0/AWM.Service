namespace AWM.Service.Application.Features.Thesis.Directions.Queries.GetDirectionById;

using AWM.Service.Application.Features.Thesis.Directions.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to get a direction by ID with full details.
/// </summary>
public sealed record GetDirectionByIdQuery : IRequest<Result<DirectionDetailDto>>
{
    /// <summary>
    /// Direction ID.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Include soft-deleted directions (default: false).
    /// </summary>
    public bool IncludeDeleted { get; init; } = false;
}
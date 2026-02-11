namespace AWM.Service.Application.Features.Thesis.Directions.Commands.UpdateDirection;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to update a research direction (only in draft state).
/// </summary>
public sealed record UpdateDirectionCommand : IRequest<Result>
{
    /// <summary>
    /// Direction ID to update.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Title in Russian (required).
    /// </summary>
    public string TitleRu { get; init; } = string.Empty;

    /// <summary>
    /// Title in Kazakh (optional).
    /// </summary>
    public string? TitleKz { get; init; }

    /// <summary>
    /// Title in English (optional).
    /// </summary>
    public string? TitleEn { get; init; }

    /// <summary>
    /// Direction description (optional).
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// User ID who modifies this direction.
    /// </summary>
    public int ModifiedBy { get; init; }
}
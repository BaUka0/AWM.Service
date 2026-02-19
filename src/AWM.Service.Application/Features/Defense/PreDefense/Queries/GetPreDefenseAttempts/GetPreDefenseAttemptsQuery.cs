namespace AWM.Service.Application.Features.Defense.PreDefense.Queries.GetPreDefenseAttempts;

using AWM.Service.Application.Features.Defense.PreDefense.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve all pre-defense attempts for a student work.
/// </summary>
public sealed record GetPreDefenseAttemptsQuery : IRequest<Result<IReadOnlyList<PreDefenseAttemptDto>>>
{
    /// <summary>
    /// StudentWork ID to retrieve attempts for.
    /// </summary>
    public long WorkId { get; init; }
}

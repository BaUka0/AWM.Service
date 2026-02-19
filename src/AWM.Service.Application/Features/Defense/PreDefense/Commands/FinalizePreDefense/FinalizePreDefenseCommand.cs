namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.FinalizePreDefense;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command for a secretary to finalize a pre-defense attempt by recording the result
/// (average score and pass/fail decision) on an existing PreDefenseAttempt.
/// </summary>
public sealed record FinalizePreDefenseCommand : IRequest<Result>
{
    /// <summary>
    /// PreDefenseAttempt ID to finalize.
    /// </summary>
    public long AttemptId { get; init; }

    /// <summary>
    /// Final average score to record.
    /// </summary>
    public decimal AverageScore { get; init; }

    /// <summary>
    /// Whether the student passed the pre-defense.
    /// </summary>
    public bool IsPassed { get; init; }
}

namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.SubmitPreDefenseGrade;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command for a commission member to submit a grade for a scheduled pre-defense slot.
/// </summary>
public sealed record SubmitPreDefenseGradeCommand : IRequest<Result<long>>
{
    /// <summary>
    /// Schedule ID (the pre-defense slot) to grade.
    /// </summary>
    public long ScheduleId { get; init; }

    /// <summary>
    /// Assignment ID (Unified Staff Assignment) of the member submitting the grade.
    /// </summary>
    public long AssignmentId { get; init; }

    /// <summary>
    /// Evaluation criteria ID being graded.
    /// </summary>
    public int CriteriaId { get; init; }

    /// <summary>
    /// Score value.
    /// </summary>
    public int Score { get; init; }

    /// <summary>
    /// Optional comment from the commission member.
    /// </summary>
    public string? Comment { get; init; }
}

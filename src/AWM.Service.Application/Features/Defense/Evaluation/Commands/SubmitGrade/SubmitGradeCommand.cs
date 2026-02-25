namespace AWM.Service.Application.Features.Defense.Evaluation.Commands.SubmitGrade;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command for a GAK commission member to submit a grade for a defense schedule slot.
/// </summary>
public sealed record SubmitGradeCommand : IRequest<Result<long>>
{
    /// <summary>
    /// Schedule ID (the defense slot) to grade.
    /// </summary>
    public long ScheduleId { get; init; }

    /// <summary>
    /// Commission member ID submitting the grade.
    /// </summary>
    public int MemberId { get; init; }

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

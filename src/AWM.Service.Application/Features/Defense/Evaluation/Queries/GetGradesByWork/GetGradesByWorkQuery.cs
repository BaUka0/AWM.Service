namespace AWM.Service.Application.Features.Defense.Evaluation.Queries.GetGradesByWork;

using AWM.Service.Application.Features.Defense.Evaluation.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Query to retrieve all grades for a defense schedule (work).
/// </summary>
public sealed record GetGradesByWorkQuery : IRequest<Result<IReadOnlyList<GradeDto>>>
{
    /// <summary>
    /// Schedule ID to retrieve grades for.
    /// </summary>
    public long ScheduleId { get; init; }
}

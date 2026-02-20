namespace AWM.Service.Application.Features.Defense.Evaluation.Queries.GetGradesByWork;

using AWM.Service.Application.Features.Defense.Evaluation.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving grades by schedule (work) ID.
/// </summary>
public sealed class GetGradesByWorkQueryHandler
    : IRequestHandler<GetGradesByWorkQuery, Result<IReadOnlyList<GradeDto>>>
{
    private readonly IScheduleRepository _scheduleRepository;

    public GetGradesByWorkQueryHandler(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
    }

    public async Task<Result<IReadOnlyList<GradeDto>>> Handle(
        GetGradesByWorkQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
            if (schedule is null)
                return Result.Failure<IReadOnlyList<GradeDto>>(new Error("NotFound.Schedule",
                    $"Schedule with ID {request.ScheduleId} not found."));

            var dtos = schedule.Grades
                .Select(g => new GradeDto
                {
                    Id = g.Id,
                    ScheduleId = g.ScheduleId,
                    MemberId = g.MemberId,
                    CriteriaId = g.CriteriaId,
                    Score = g.Score,
                    Comment = g.Comment,
                    GradedAt = g.GradedAt
                })
                .ToList();

            return Result.Success<IReadOnlyList<GradeDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<GradeDto>>(
                new Error("InternalError", $"An error occurred: {ex.Message}"));
        }
    }
}

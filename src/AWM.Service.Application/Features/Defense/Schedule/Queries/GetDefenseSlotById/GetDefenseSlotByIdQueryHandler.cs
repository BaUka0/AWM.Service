namespace AWM.Service.Application.Features.Defense.Schedule.Queries.GetDefenseSlotById;

using AWM.Service.Application.Features.Defense.Evaluation.DTOs;
using AWM.Service.Application.Features.Defense.Schedule.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving a single defense slot by ID with grades.
/// </summary>
public sealed class GetDefenseSlotByIdQueryHandler
    : IRequestHandler<GetDefenseSlotByIdQuery, Result<DefenseSlotDto>>
{
    private readonly IScheduleRepository _scheduleRepository;

    public GetDefenseSlotByIdQueryHandler(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
    }

    public async Task<Result<DefenseSlotDto>> Handle(
        GetDefenseSlotByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var schedule = await _scheduleRepository.GetByIdAsync(request.SlotId, cancellationToken);
            if (schedule is null)
                return Result.Failure<DefenseSlotDto>(new Error("NotFound.Schedule",
                    $"Schedule with ID {request.SlotId} not found."));

            var dto = new DefenseSlotDto
            {
                Id = schedule.Id,
                CommissionId = schedule.CommissionId,
                WorkId = schedule.WorkId,
                DefenseDate = schedule.DefenseDate,
                Location = schedule.Location,
                AverageScore = schedule.GetAverageScore(),
                Grades = schedule.Grades.Select(g => new GradeDto
                {
                    Id = g.Id,
                    ScheduleId = g.ScheduleId,
                    MemberId = g.MemberId,
                    CriteriaId = g.CriteriaId,
                    Score = g.Score,
                    Comment = g.Comment,
                    GradedAt = g.GradedAt
                }).ToList(),
                CreatedAt = schedule.CreatedAt
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<DefenseSlotDto>(
                new Error("InternalError", $"An error occurred: {ex.Message}"));
        }
    }
}

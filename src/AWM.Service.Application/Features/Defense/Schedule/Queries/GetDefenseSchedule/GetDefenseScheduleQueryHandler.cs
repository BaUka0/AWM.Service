namespace AWM.Service.Application.Features.Defense.Schedule.Queries.GetDefenseSchedule;

using AWM.Service.Application.Features.Defense.Schedule.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving the defense schedule of a GAK commission.
/// </summary>
public sealed class GetDefenseScheduleQueryHandler
    : IRequestHandler<GetDefenseScheduleQuery, Result<IReadOnlyList<ScheduleDto>>>
{
    private readonly IScheduleRepository _scheduleRepository;

    public GetDefenseScheduleQueryHandler(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
    }

    public async Task<Result<IReadOnlyList<ScheduleDto>>> Handle(
        GetDefenseScheduleQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var schedules = await _scheduleRepository.GetByCommissionAsync(request.CommissionId, cancellationToken);

            var dtos = schedules
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.DefenseDate)
                .Select(s => new ScheduleDto
                {
                    Id = s.Id,
                    CommissionId = s.CommissionId,
                    WorkId = s.WorkId,
                    DefenseDate = s.DefenseDate,
                    Location = s.Location,
                    AverageScore = s.GetAverageScore(),
                    GradeCount = s.Grades.Count,
                    CreatedAt = s.CreatedAt
                })
                .ToList();

            return Result.Success<IReadOnlyList<ScheduleDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<ScheduleDto>>(
                new Error("InternalError", $"An error occurred: {ex.Message}"));
        }
    }
}

namespace AWM.Service.Application.Features.Defense.PreDefense.Queries.GetPreDefenseSchedule;

using AWM.Service.Application.Features.Defense.PreDefense.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving the schedule of a pre-defense commission.
/// </summary>
public sealed class GetPreDefenseScheduleQueryHandler
    : IRequestHandler<GetPreDefenseScheduleQuery, Result<IReadOnlyList<PreDefenseScheduleDto>>>
{
    private readonly IScheduleRepository _scheduleRepository;

    public GetPreDefenseScheduleQueryHandler(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
    }

    public async Task<Result<IReadOnlyList<PreDefenseScheduleDto>>> Handle(
        GetPreDefenseScheduleQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var schedules = await _scheduleRepository.GetByCommissionAsync(request.CommissionId, cancellationToken);

            var dtos = schedules
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.DefenseDate)
                .Select(s => new PreDefenseScheduleDto
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

            return Result.Success<IReadOnlyList<PreDefenseScheduleDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<PreDefenseScheduleDto>>(
                new Error("InternalError", $"An error occurred: {ex.Message}"));
        }
    }
}

using AWM.Service.Application.Features.Defense.Schedules.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Schedules.Queries.GetScheduleGrades;

public sealed class GetScheduleGradesQueryHandler : IRequestHandler<GetScheduleGradesQuery, Result<IReadOnlyList<GradeDto>>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUserRepository _userRepository;

    public GetScheduleGradesQueryHandler(
        IScheduleRepository scheduleRepository,
        ICommissionRepository commissionRepository,
        IUserRepository userRepository)
    {
        _scheduleRepository = scheduleRepository;
        _commissionRepository = commissionRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<IReadOnlyList<GradeDto>>> Handle(GetScheduleGradesQuery request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        if (schedule == null)
        {
            return Result.Failure<IReadOnlyList<GradeDto>>(new Error("Schedule.NotFound", $"Schedule with ID {request.ScheduleId} not found."));
        }

        var commission = await _commissionRepository.GetByIdWithAssignmentsAsync(schedule.CommissionId, cancellationToken);
        if (commission == null)
        {
            return Result.Failure<IReadOnlyList<GradeDto>>(new Error("Commission.NotFound", "Commission for this schedule not found."));
        }

        var userIds = commission.Assignments.Select(a => a.UserId).Distinct();
        var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);

        var result = new List<GradeDto>();
        foreach (var grade in schedule.Grades)
        {
            var assignment = commission.Assignments.FirstOrDefault(a => a.Id == grade.AssignmentId);
            var user = users.FirstOrDefault(u => u.Id == (assignment?.UserId ?? 0));
            var memberName = user != null ? $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim() : "Unknown";

            result.Add(new GradeDto(
                grade.Id,
                grade.ScheduleId,
                grade.AssignmentId,
                grade.CriteriaId,
                grade.Score,
                grade.Comment,
                memberName,
                user?.Id));
        }

        return Result.Success<IReadOnlyList<GradeDto>>(result);
    }
}

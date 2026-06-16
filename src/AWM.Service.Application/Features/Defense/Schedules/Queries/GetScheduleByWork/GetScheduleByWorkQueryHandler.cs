using AWM.Service.Application.Features.Defense.Schedules.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Defense.Schedules.Queries.GetScheduleByWork;

public sealed class GetScheduleByWorkQueryHandler : IRequestHandler<GetScheduleByWorkQuery, Result<ScheduleByWorkDto?>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUserRepository _userRepository;

    public GetScheduleByWorkQueryHandler(
        IScheduleRepository scheduleRepository,
        ICommissionRepository commissionRepository,
        IUserRepository userRepository)
    {
        _scheduleRepository = scheduleRepository;
        _commissionRepository = commissionRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<ScheduleByWorkDto?>> Handle(GetScheduleByWorkQuery request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);
        if (schedule == null)
        {
            return Result.Success<ScheduleByWorkDto?>(null);
        }

        var commission = await _commissionRepository.GetByIdWithAssignmentsAsync(schedule.CommissionId, cancellationToken);
        IReadOnlyList<CommissionMemberInfoDto> members = new List<CommissionMemberInfoDto>();
        string? commissionName = null;

        if (commission != null)
        {
            commissionName = commission.Name;
            var userIds = commission.Assignments.Where(a => a.IsActive && !a.IsDeleted).Select(a => a.UserId).Distinct();
            var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);

            members = commission.Assignments
                .Where(a => a.IsActive && !a.IsDeleted)
                .Select(a =>
                {
                    var user = users.FirstOrDefault(u => u.Id == a.UserId);
                    var name = user != null ? $"{user.LastName} {user.FirstName}".Trim() : "Unknown";
                    return new CommissionMemberInfoDto(a.RoleType.ToString(), name);
                })
                .ToList();
        }

        var dto = new ScheduleByWorkDto(
            schedule.Id,
            schedule.DefenseDate.ToString("dd.MM.yyyy"),
            schedule.DefenseDate.ToString("HH:mm"),
            schedule.Location,
            commission?.Id,
            commissionName,
            members,
            schedule.IsReconciliationStarted,
            schedule.GetAverageScore()
        );

        return Result.Success<ScheduleByWorkDto?>(dto);
    }
}

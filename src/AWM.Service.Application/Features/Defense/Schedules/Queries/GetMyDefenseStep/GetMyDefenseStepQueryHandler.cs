using AWM.Service.Application.Features.Defense.Schedules.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Schedules.Queries.GetMyDefenseStep;

public sealed class GetMyDefenseStepQueryHandler : IRequestHandler<GetMyDefenseStepQuery, Result<DefenseStepDto>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IProtocolRepository _protocolRepository;
    private readonly IPreDefenseAttemptRepository _attemptRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetMyDefenseStepQueryHandler(
        IStudentWorkRepository workRepository,
        IScheduleRepository scheduleRepository,
        ICommissionRepository commissionRepository,
        IProtocolRepository protocolRepository,
        IPreDefenseAttemptRepository attemptRepository,
        IUserRepository userRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository;
        _scheduleRepository = scheduleRepository;
        _commissionRepository = commissionRepository;
        _protocolRepository = protocolRepository;
        _attemptRepository = attemptRepository;
        _userRepository = userRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<DefenseStepDto>> Handle(GetMyDefenseStepQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId ?? 0;
        if (currentUserId == 0)
        {
            return Result.Failure<DefenseStepDto>(new Error("Auth.Unauthorized", "User not authenticated."));
        }

        // 1. Find student's work
        var works = await _workRepository.GetByStudentAsync(currentUserId, cancellationToken);
        var work = works.FirstOrDefault();
        if (work == null)
        {
            return Result.Failure<DefenseStepDto>(new Error("Work.NotFound", "Student work not found."));
        }

        // 2. Find schedule
        var schedule = await _scheduleRepository.GetByWorkIdAsync(work.Id, cancellationToken);
        ScheduleInfoDto? scheduleInfo = null;
        IReadOnlyList<CommissionMemberInfoDto> commissionMembers = new List<CommissionMemberInfoDto>();

        if (schedule != null)
        {
            scheduleInfo = new ScheduleInfoDto(
                schedule.DefenseDate.ToShortDateString(),
                schedule.DefenseDate.ToShortTimeString(),
                schedule.Location ?? "Online"
            );

            // 3. Find commission members
            var commission = await _commissionRepository.GetByIdWithAssignmentsAsync(schedule.CommissionId, cancellationToken);
            if (commission != null)
            {
                var userIds = commission.Assignments.Select(a => a.UserId).Distinct();
                var users = await _userRepository.GetByIdsAsync(userIds, cancellationToken);

                commissionMembers = commission.Assignments
                    .Where(a => a.IsActive && !a.IsDeleted)
                    .Select(a => {
                        var user = users.FirstOrDefault(u => u.Id == a.UserId);
                        var name = user != null ? $"{user.LastName} {user.FirstName}" : "Unknown";
                        return new CommissionMemberInfoDto(a.RoleType.ToString(), name);
                    })
                    .ToList();
            }
        }

        // 4. Previous attempts (Pre-defenses)
        var attempts = await _attemptRepository.GetByWorkIdAsync(work.Id, cancellationToken);
        var previousAttempts = new List<AttemptHistoryDto>();
        
        foreach (var attempt in attempts)
        {
            string? comments = null;
            if (attempt.ScheduleId.HasValue)
            {
                var attemptProtocol = await _protocolRepository.GetByScheduleIdAsync(attempt.ScheduleId.Value, cancellationToken);
                comments = attemptProtocol?.Comments;
            }

            previousAttempts.Add(new AttemptHistoryDto(
                attempt.PreDefenseNumber,
                attempt.IsPassed,
                attempt.AverageScore ?? 0,
                attempt.AttemptDate.ToShortDateString(),
                comments
            ));
        }

        // 5. Results (if protocol exists)
        DefenseResultsDto? results = null;
        var protocol = await _protocolRepository.GetByScheduleIdAsync(schedule?.Id ?? 0, cancellationToken);
        if (protocol != null)
        {
            results = new DefenseResultsDto(
                protocol.Decision == "Допущен", // Simplified
                protocol.FinalScoreNumeric ?? 0,
                protocol.FinalGradeLetter ?? "-",
                protocol.Decision ?? "-",
                protocol.SessionDate.ToShortDateString()
            );
        }

        return Result.Success(new DefenseStepDto(
            schedule != null ? "defense" : "pre-defense", // Match frontend expectation
            previousAttempts.Count + 1,
            scheduleInfo,
            commissionMembers,
            previousAttempts,
            results
        ));
    }
}

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.AutoDistributeStudents;

public sealed class AutoDistributeStudentsCommandHandler : IRequestHandler<AutoDistributeStudentsCommand, Result>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStageRepository _stageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public AutoDistributeStudentsCommandHandler(
        IStudentWorkRepository studentWorkRepository,
        ICommissionRepository commissionRepository,
        IScheduleRepository scheduleRepository,
        IWorkflowRepository workflowRepository,
        IUserRepository userRepository,
        IStageRepository stageRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _studentWorkRepository = studentWorkRepository;
        _commissionRepository = commissionRepository;
        _scheduleRepository = scheduleRepository;
        _workflowRepository = workflowRepository;
        _userRepository = userRepository;
        _stageRepository = stageRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(AutoDistributeStudentsCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var waitingStateIds = new List<int>();
        var workTypes = await _workflowRepository.GetAllWorkTypesAsync(cancellationToken);

        if (request.CommissionTypeId == 1)
        {
            int pdNum = request.PreDefenseNumber ?? 1;

            if (pdNum == 1)
            {
                foreach (var wt in workTypes)
                {
                    var state = await _workflowRepository.GetStateBySystemNameAsync(wt.Id, WorkStates.Draft, cancellationToken);
                    if (state != null)
                    {
                        waitingStateIds.Add(state.Id);
                    }
                }
            }

            string stateName = pdNum switch
            {
                1 => WorkStates.PreDefense1WaitingForSchedule,
                2 => WorkStates.PreDefense2WaitingForSchedule,
                3 => WorkStates.PreDefense3WaitingForSchedule,
                _ => WorkStates.PreDefense1WaitingForSchedule
            };

            foreach (var wt in workTypes)
            {
                var state = await _workflowRepository.GetStateBySystemNameAsync(wt.Id, stateName, cancellationToken);
                if (state != null)
                {
                    waitingStateIds.Add(state.Id);
                }
            }
        }
        else if (request.CommissionTypeId == 2)
        {
            foreach (var wt in workTypes)
            {
                var state1 = await _workflowRepository.GetStateBySystemNameAsync(wt.Id, WorkStates.DefenseWaitingForSchedule, cancellationToken);
                if (state1 != null) waitingStateIds.Add(state1.Id);

                var state2 = await _workflowRepository.GetStateBySystemNameAsync(wt.Id, WorkStates.ReadyForDefense, cancellationToken);
                if (state2 != null) waitingStateIds.Add(state2.Id);
            }
        }

        if (!waitingStateIds.Any())
        {
            return Result.Failure(new Error("AutoDistribute.StatesNotFound", "Could not resolve workflow states for distribution."));
        }

        var allWorks = await _studentWorkRepository.GetByOrgUnitAsync(request.OrgUnitId, request.SemesterId, cancellationToken);

        var worksToDistribute = allWorks
            .Where(w => !w.IsDeleted && waitingStateIds.Contains(w.CurrentStateId))
            .DistinctBy(w => w.Id)
            .ToList();

        if (request.SpecialityId.HasValue)
        {
            worksToDistribute = worksToDistribute.Where(w => w.SpecialityId == request.SpecialityId.Value).ToList();
        }

        if (!worksToDistribute.Any())
        {
            return Result.Failure(new Error("AutoDistribute.NoStudents", "Нет студентов, ожидающих распределения на данном этапе."));
        }

        var allCommissions = await _commissionRepository.GetByTypeAsync(
            request.OrgUnitId,
            request.SemesterId,
            request.CommissionTypeId,
            cancellationToken);

        var targetCommissions = allCommissions.Where(c => !c.IsDeleted).ToList();

        if (request.CommissionTypeId == 1)
        {
            targetCommissions = targetCommissions.Where(c => c.PreDefenseNumber == request.PreDefenseNumber).ToList();
        }

        if (request.SpecialityId.HasValue)
        {
            var specCommissions = targetCommissions.Where(c => c.SpecialityId == request.SpecialityId.Value).ToList();
            if (specCommissions.Any())
            {
                targetCommissions = specCommissions;
            }
            else
            {
                targetCommissions = targetCommissions.Where(c => !c.SpecialityId.HasValue).ToList();
            }
        }
        else
        {
            targetCommissions = targetCommissions.Where(c => !c.SpecialityId.HasValue).ToList();
        }

        if (!targetCommissions.Any())
        {
            return Result.Failure(new Error("AutoDistribute.NoCommissions", "No active commissions found for student distribution."));
        }

        var studentUserIds = worksToDistribute.SelectMany(w => w.Participants.Select(p => p.StudentId)).Distinct().ToList();
        var users = await _userRepository.GetByIdsAsync(studentUserIds, cancellationToken);
        var userMap = users.ToDictionary(u => u.Id);

        var sortedWorks = worksToDistribute
            .OrderBy(w => w.SpecialityId)
            .ThenBy(w =>
            {
                var firstParticipant = w.Participants.FirstOrDefault();
                if (firstParticipant != null && userMap.TryGetValue(firstParticipant.StudentId, out var user))
                {
                    return $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim();
                }
                return string.Empty;
            })
            .ToList();

        int workflowStageId = request.CommissionTypeId == 2 ? 8 : (4 + (request.PreDefenseNumber ?? 1));
        var stage = await _stageRepository.GetActiveByStageAsync(request.OrgUnitId, request.SemesterId, workflowStageId, request.SpecialityId, cancellationToken);
        stage ??= await _stageRepository.GetActiveByStageAsync(request.OrgUnitId, request.SemesterId, workflowStageId, null, cancellationToken);

        var baseTime = stage?.StartDate ?? DateTime.UtcNow.Date.AddDays(1).AddHours(9);
        if (baseTime < DateTime.UtcNow)
        {
            baseTime = DateTime.UtcNow.Date.AddDays(1).AddHours(9);
        }

        var sortedCommissions = targetCommissions.OrderBy(c => c.Id).ToList();
        var commissionNextSlotTime = new Dictionary<int, DateTime>();
        var alreadyScheduledWorkIds = new HashSet<long>();

        foreach (var c in sortedCommissions)
        {
            var existingSchedules = await _scheduleRepository.GetByCommissionAsync(c.Id, cancellationToken);
            var activeSchedules = existingSchedules.Where(s => !s.IsDeleted).ToList();

            foreach (var s in activeSchedules)
            {
                alreadyScheduledWorkIds.Add(s.WorkId);
            }

            var lastSlot = activeSchedules.OrderByDescending(s => s.DefenseDate).FirstOrDefault();

            var nextTime = lastSlot != null ? lastSlot.DefenseDate.AddMinutes(30) : baseTime;
            if (nextTime < DateTime.UtcNow)
            {
                nextTime = DateTime.UtcNow.Date.AddDays(1).AddHours(9);
            }
            commissionNextSlotTime[c.Id] = nextTime;
        }

        int totalCommissions = sortedCommissions.Count;
        for (int i = 0; i < sortedWorks.Count; i++)
        {
            var work = sortedWorks[i];

            if (alreadyScheduledWorkIds.Contains(work.Id))
            {
                await TransitionWorkState(work, "Already scheduled", null, currentUserId, cancellationToken);
                continue;
            }

            var commission = sortedCommissions[i % totalCommissions];
            var slotTime = commissionNextSlotTime[commission.Id];

            var schedule = new Schedule(
                commission.Id,
                work.Id,
                slotTime,
                currentUserId,
                "Кафедра"
            );
            await _scheduleRepository.AddAsync(schedule, cancellationToken);

            await TransitionWorkState(work, commission.Name ?? "Unknown", slotTime, currentUserId, cancellationToken);

            commissionNextSlotTime[commission.Id] = slotTime.AddMinutes(30);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task TransitionWorkState(
        StudentWork work,
        string? commissionName,
        DateTime? slotTime,
        int currentUserId,
        CancellationToken cancellationToken)
    {
        var currentState = await _workflowRepository.GetStateByIdAsync(work.CurrentStateId, cancellationToken);
        if (currentState == null) return;

        string? targetStateName = null;
        if (currentState.SystemName == WorkStates.PreDefense1WaitingForSchedule || currentState.SystemName == WorkStates.Draft)
            targetStateName = WorkStates.PreDefense1Scheduled;
        else if (currentState.SystemName == WorkStates.PreDefense2WaitingForSchedule)
            targetStateName = WorkStates.PreDefense2Scheduled;
        else if (currentState.SystemName == WorkStates.PreDefense3WaitingForSchedule)
            targetStateName = WorkStates.PreDefense3Scheduled;
        else if (currentState.SystemName == WorkStates.ReadyForDefense || currentState.SystemName == WorkStates.DefenseWaitingForSchedule)
            targetStateName = WorkStates.DefenseScheduled;

        if (targetStateName != null)
        {
            var targetState = await _workflowRepository.GetStateBySystemNameAsync(currentState.WorkTypeId, targetStateName, cancellationToken);
            if (targetState != null)
            {
                string msg = slotTime.HasValue
                    ? $"Automatically distributed to commission '{commissionName ?? "Unknown"}' at {slotTime.Value}."
                    : $"Automatically moved to scheduled state (existing schedule found).";

                work.ChangeState(targetState.Id, currentUserId, msg);
                await _studentWorkRepository.UpdateAsync(work, cancellationToken);
            }
        }
    }
}

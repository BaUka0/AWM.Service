using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.GenerateSchedule;

public sealed class GenerateScheduleCommandHandler : IRequestHandler<GenerateScheduleCommand, Result>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GenerateScheduleCommandHandler(
        IScheduleRepository scheduleRepository,
        IStudentWorkRepository studentWorkRepository,
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _scheduleRepository = scheduleRepository;
        _studentWorkRepository = studentWorkRepository;
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(GenerateScheduleCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId ?? 0;
        var currentSlotTime = request.StartDate;

        try
        {
            foreach (var workId in request.WorkIds)
            {
                var schedule = new Schedule(
                    request.CommissionId,
                    workId,
                    currentSlotTime,
                    currentUserId,
                    request.Location);

                await _scheduleRepository.AddAsync(schedule, cancellationToken);

                var work = await _studentWorkRepository.GetByIdWithDetailsAsync(workId, cancellationToken);
                if (work != null)
                {
                    var currentState = await _workflowRepository.GetStateByIdAsync(work.CurrentStateId, cancellationToken);
                    if (currentState != null)
                    {
                        string? targetStateName = null;
                        if (currentState.SystemName == WorkStates.PreDefense1WaitingForSchedule)
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
                                work.ChangeState(targetState.Id, currentUserId, $"Scheduled for defense session on {currentSlotTime}.");
                                await _studentWorkRepository.UpdateAsync(work, cancellationToken);
                            }
                        }
                    }
                }

                currentSlotTime = currentSlotTime.AddMinutes(request.SlotDurationMinutes);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(new Error(ex.ErrorCode, ex.Message));
        }
    }
}

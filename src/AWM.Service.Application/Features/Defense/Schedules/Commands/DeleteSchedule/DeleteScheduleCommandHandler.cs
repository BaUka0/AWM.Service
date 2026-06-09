using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Defense.Schedules.Commands.DeleteSchedule;

/// <summary>
/// Command handler for DeleteScheduleCommand.
/// Soft-deletes a schedule slot and transitions the student work back to its corresponding waiting state.
/// </summary>
public sealed class DeleteScheduleCommandHandler : IRequestHandler<DeleteScheduleCommand, Result>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public DeleteScheduleCommandHandler(
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

    public async Task<Result> Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var deletedBy = _currentUserProvider.UserId.Value;

        var schedule = await _scheduleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (schedule == null)
        {
            return Result.Failure(new Error("Schedule.NotFound", $"Schedule slot with ID {request.Id} not found."));
        }

        schedule.Delete(deletedBy);
        await _scheduleRepository.UpdateAsync(schedule, cancellationToken);

        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(schedule.WorkId, cancellationToken);
        if (work != null)
        {
            var currentState = await _workflowRepository.GetStateByIdAsync(work.CurrentStateId, cancellationToken);
            if (currentState != null)
            {
                string? targetStateName = null;
                if (currentState.SystemName == WorkStates.PreDefense1Scheduled)
                    targetStateName = WorkStates.PreDefense1WaitingForSchedule;
                else if (currentState.SystemName == WorkStates.PreDefense2Scheduled)
                    targetStateName = WorkStates.PreDefense2WaitingForSchedule;
                else if (currentState.SystemName == WorkStates.PreDefense3Scheduled)
                    targetStateName = WorkStates.PreDefense3WaitingForSchedule;
                else if (currentState.SystemName == WorkStates.DefenseScheduled)
                    targetStateName = WorkStates.DefenseWaitingForSchedule;

                if (targetStateName != null)
                {
                    var targetState = await _workflowRepository.GetStateBySystemNameAsync(currentState.WorkTypeId, targetStateName, cancellationToken);
                    if (targetState != null)
                    {
                        work.ChangeState(targetState.Id, deletedBy, "Defense schedule slot cancelled / deleted manually.");
                        await _studentWorkRepository.UpdateAsync(work, cancellationToken);
                    }
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

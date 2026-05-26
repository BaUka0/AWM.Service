using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Defense.Enums;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Defense.Protocols.Commands.FinalizeProtocol;

public sealed class FinalizeProtocolCommandHandler : IRequestHandler<FinalizeProtocolCommand, Result>
{
    private readonly IProtocolRepository _protocolRepository;
    private readonly ICommissionRepository _commissionRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IPreDefenseAttemptRepository _preDefenseAttemptRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public FinalizeProtocolCommandHandler(
        IProtocolRepository protocolRepository,
        ICommissionRepository commissionRepository,
        IScheduleRepository scheduleRepository,
        IStudentWorkRepository studentWorkRepository,
        IPreDefenseAttemptRepository preDefenseAttemptRepository,
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _protocolRepository = protocolRepository;
        _commissionRepository = commissionRepository;
        _scheduleRepository = scheduleRepository;
        _studentWorkRepository = studentWorkRepository;
        _preDefenseAttemptRepository = preDefenseAttemptRepository;
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }


    public async Task<Result> Handle(FinalizeProtocolCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.IsAuthenticated || !_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.MissingPrincipal", "Unable to identify the current user."));

        var currentUserId = _currentUserProvider.UserId.Value;

        var protocol = await _protocolRepository.GetByIdAsync(request.ProtocolId, cancellationToken);
        if (protocol == null)
            return Result.Failure(new Error("Protocol.NotFound", $"Protocol with ID {request.ProtocolId} not found."));

        // Only the commission chairman or secretary may finalize
        var commission = await _commissionRepository.GetByIdWithAssignmentsAsync(protocol.CommissionId, cancellationToken);
        if (commission == null)
            return Result.Failure(new Error("Commission.NotFound", "Commission for this protocol not found."));

        var userAssignment = commission.Assignments
            .FirstOrDefault(a => a.UserId == currentUserId && a.IsActive && !a.IsDeleted);

        if (userAssignment == null ||
            (userAssignment.RoleType != StaffRoleType.CommissionChairman &&
             userAssignment.RoleType != StaffRoleType.CommissionSecretary))
        {
            return Result.Failure(new Error("Commission.Unauthorized",
                "Only the chairman or secretary of the commission can finalize the protocol."));
        }

        try
        {
            protocol.Finalize(currentUserId);

            // Retrieve Schedule and StudentWork
            var schedule = await _scheduleRepository.GetByIdAsync(protocol.ScheduleId, cancellationToken);
            if (schedule == null)
                return Result.Failure(new Error("Schedule.NotFound", "Schedule not found."));

            var work = await _studentWorkRepository.GetByIdWithDetailsAsync(schedule.WorkId, cancellationToken);
            if (work == null)
                return Result.Failure(new Error("StudentWork.NotFound", "Student work not found."));

            // Determine if GAK or PreDefense
            if (commission.CommissionTypeId == (int)CommissionTypes.PreDefense)
            {
                var preDefenseNum = commission.PreDefenseNumber ?? 1;

                // Load or create PreDefenseAttempt
                var attempts = await _preDefenseAttemptRepository.GetByWorkIdAsync(work.Id, cancellationToken);
                var attempt = attempts.FirstOrDefault(a => a.PreDefenseNumber == preDefenseNum && a.ScheduleId == schedule.Id);
                if (attempt == null)
                {
                    attempt = new PreDefenseAttempt(work.Id, preDefenseNum, currentUserId, schedule.Id);
                    await _preDefenseAttemptRepository.AddAsync(attempt, cancellationToken);
                }

                // Any pre-defense finalized protocol is considered passing in terms of score check,
                // and passing is determined by decision not being 'Не допущен' (except PreDefense 1 which always passes)
                bool isPassed = true;
                if (preDefenseNum > 1)
                {
                    isPassed = !string.Equals(protocol.Decision, "Не допущен", StringComparison.OrdinalIgnoreCase);
                }

                attempt.RecordResult(protocol.FinalScoreNumeric ?? 0, isPassed, currentUserId);
                await _preDefenseAttemptRepository.UpdateAsync(attempt, cancellationToken);

                // Transition student work state
                string? targetStateName = null;
                if (preDefenseNum == 1)
                {
                    targetStateName = WorkStates.PreDefense2WaitingForFiles;
                }
                else if (preDefenseNum == 2)
                {
                    targetStateName = isPassed ? WorkStates.ChecksWaitingForInitial : WorkStates.PreDefense3WaitingForFiles;
                }
                else if (preDefenseNum == 3)
                {
                    targetStateName = isPassed ? WorkStates.ChecksWaitingForInitial : WorkStates.Cancelled;
                }

                if (targetStateName != null)
                {
                    var currentState = await _workflowRepository.GetStateByIdAsync(work.CurrentStateId, cancellationToken);
                    if (currentState != null)
                    {
                        var targetState = await _workflowRepository.GetStateBySystemNameAsync(currentState.WorkTypeId, targetStateName, cancellationToken);
                        if (targetState != null)
                        {
                            work.ChangeState(targetState.Id, currentUserId, $"Finalized Pre-Defense {preDefenseNum} with status: {(isPassed ? "Passed" : "Failed")}.");
                            await _studentWorkRepository.UpdateAsync(work, cancellationToken);
                        }
                    }
                }
            }
            else if (commission.CommissionTypeId == (int)CommissionTypes.GAK)
            {
                bool isPassed = !string.Equals(protocol.Decision, "Не допущен", StringComparison.OrdinalIgnoreCase);

                string? targetStateName = isPassed ? WorkStates.Defended : WorkStates.DefenseFailed;

                var currentState = await _workflowRepository.GetStateByIdAsync(work.CurrentStateId, cancellationToken);
                if (currentState != null && targetStateName != null)
                {
                    var targetState = await _workflowRepository.GetStateBySystemNameAsync(currentState.WorkTypeId, targetStateName, cancellationToken);
                    if (targetState != null)
                    {
                        if (isPassed)
                        {
                            work.MarkAsDefended(protocol.FinalGradeLetter);
                        }
                        work.ChangeState(targetState.Id, currentUserId, $"Finalized Defense (GAK) with status: {(isPassed ? "Defended" : "Failed")}.");
                        await _studentWorkRepository.UpdateAsync(work, cancellationToken);
                    }
                }
            }

            await _protocolRepository.UpdateAsync(protocol, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(new Error(ex.ErrorCode, ex.Message));
        }
    }
}

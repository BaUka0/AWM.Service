namespace AWM.Service.Application.Features.Workflow.Commands.TransitionState;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Wf.Services;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class TransitionStateCommandHandler : IRequestHandler<TransitionStateCommand, Result>
{
    private readonly IStateMachine _stateMachine;
    private readonly IDirectionRepository _directionRepository;
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ISpecialityCheckTypeRepository _specialityCheckTypeRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IPreDefenseAttemptRepository _preDefenseAttemptRepository;
    private readonly IStageValidationService _stageValidationService;
    private readonly IEmployeeRepository _employeeRepository;

    public TransitionStateCommandHandler(
        IStateMachine stateMachine,
        IDirectionRepository directionRepository,
        IStudentWorkRepository studentWorkRepository,
        ICurrentUserProvider currentUserProvider,
        ISpecialityCheckTypeRepository specialityCheckTypeRepository,
        IStudentRepository studentRepository,
        IPreDefenseAttemptRepository preDefenseAttemptRepository,
        IStageValidationService stageValidationService,
        IEmployeeRepository employeeRepository)
    {
        _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
        _studentWorkRepository = studentWorkRepository ?? throw new ArgumentNullException(nameof(studentWorkRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _specialityCheckTypeRepository = specialityCheckTypeRepository ?? throw new ArgumentNullException(nameof(specialityCheckTypeRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _preDefenseAttemptRepository = preDefenseAttemptRepository ?? throw new ArgumentNullException(nameof(preDefenseAttemptRepository));
        _stageValidationService = stageValidationService ?? throw new ArgumentNullException(nameof(stageValidationService));
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    public async Task<Result> Handle(TransitionStateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            return request.EntityType.ToLowerInvariant() switch
            {
                WorkflowEntityTypes.Direction => await TransitionDirectionAsync(request, userId.Value, cancellationToken),
                WorkflowEntityTypes.StudentWork or WorkflowEntityTypes.Work => await TransitionStudentWorkAsync(request, userId.Value, cancellationToken),
                _ => Result.Failure(new Error("400", $"Unknown entity type: {request.EntityType}"))
            };
        }
        catch (InvalidOperationException opEx)
        {
            return Result.Failure(new Error("409", opEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred during state transition: {ex.Message}"));
        }
    }

    private async Task<Result> TransitionDirectionAsync(TransitionStateCommand request, int userId, CancellationToken cancellationToken)
    {
        var direction = await _directionRepository.GetByIdAsync(request.EntityId, cancellationToken);
        if (direction is null || direction.IsDeleted)
            return Result.Failure(new Error("404", $"Direction with ID {request.EntityId} not found."));

        var targetState = await _stateMachine.GetStateAsync(request.TargetStateId, cancellationToken);
        if (targetState is null)
            return Result.Failure(new Error("404", $"Target state with ID {request.TargetStateId} not found."));

        // 1. Authorization & Business Rules based on Target State
        if (targetState.SystemName == Domain.Wf.Entities.DirectionStates.Submitted)
        {
            // Only owner can submit
            var staff = await _employeeRepository.GetByUserIdAsync(userId, cancellationToken);
            if (staff == null || direction.EmployeeId != staff.Id)
                return Result.Failure(new Error("403", "Only the supervisor who created this direction can submit it."));

            // Check if submission stage is open
            var (isAllowed, errorMessage) = await _stageValidationService
                .ValidateOperationInStageAsync(direction.OrgUnitId, direction.SemesterId, 1, null, cancellationToken);
            if (!isAllowed)
                return Result.Failure(new Error("409", errorMessage!));
        }

        var canTransition = await _stateMachine.CanTransitionAsync(direction.CurrentStateId, request.TargetStateId, null, cancellationToken);
        if (!canTransition)
            return Result.Failure(new Error("409", $"Transition from current state to state {targetState.SystemName} is not allowed."));

        // Apply the appropriate domain method based on target state
        switch (targetState.SystemName)
        {
            case Domain.Wf.Entities.DirectionStates.Submitted:
                direction.Submit(request.TargetStateId);
                break;
            case Domain.Wf.Entities.DirectionStates.Approved:
                direction.Approve(request.TargetStateId, userId);
                break;
            case Domain.Wf.Entities.DirectionStates.Rejected:
                direction.Reject(request.TargetStateId, userId, request.Comment);
                break;
            case Domain.Wf.Entities.DirectionStates.RequiresRevision:
                direction.RequestRevision(request.TargetStateId, userId, request.Comment ?? "Revision required");
                break;
            default:
                return Result.Failure(new Error("409", $"Unsupported target state: {targetState.SystemName}"));
        }

        await _directionRepository.UpdateAsync(direction, cancellationToken);
        return Result.Success();
    }

    private async Task<Result> TransitionStudentWorkAsync(TransitionStateCommand request, int userId, CancellationToken cancellationToken)
    {
        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(request.EntityId, cancellationToken);
        if (work is null || work.IsDeleted)
            return Result.Failure(new Error("404", $"StudentWork with ID {request.EntityId} not found."));

        var canTransition = await _stateMachine.CanTransitionAsync(work.CurrentStateId, request.TargetStateId, null, cancellationToken);
        if (!canTransition)
            return Result.Failure(new Error("409", $"Transition from current state to state {request.TargetStateId} is not allowed."));

        var targetState = await _stateMachine.GetStateAsync(request.TargetStateId, cancellationToken);
        if (targetState is null)
            return Result.Failure(new Error("404", $"Target state with ID {request.TargetStateId} not found."));

        // Pre-defense workflow logic
        var attempts = await _preDefenseAttemptRepository.GetByWorkIdAsync(work.Id, cancellationToken);

        // 1. Gateway to Quality Checks (Checks.WaitingForInitial)
        if (targetState.SystemName == Domain.Wf.Entities.WorkStates.ChecksWaitingForInitial)
        {
            var hasPassedDecisivePreDefense = attempts.Any(a => a.IsPassed && (a.PreDefenseNumber == 2 || a.PreDefenseNumber == 3));
            if (!hasPassedDecisivePreDefense)
            {
                return Result.Failure(new Error("BusinessRule.PreDefense", 
                    "Cannot proceed to quality checks without passing either Pre-Defense 2 or Pre-Defense 3."));
            }
        }

        // 2. Logic for Pre-Defense transitions (checking results of previous attempts)
        switch (targetState.SystemName)
        {
            case Domain.Wf.Entities.WorkStates.PreDefense2WaitingForFiles:
            case Domain.Wf.Entities.WorkStates.PreDefense2WaitingForSchedule:
                // Moving to PD2 always allowed after PD1 (no check needed here, canTransition already verified via StateMachine graph)
                break;

            case Domain.Wf.Entities.WorkStates.PreDefense3WaitingForFiles:
            case Domain.Wf.Entities.WorkStates.PreDefense3WaitingForSchedule:
                // Moving to PD3 usually after failed PD2
                var pd2Attempt = attempts.FirstOrDefault(a => a.PreDefenseNumber == 2);
                if (pd2Attempt != null && pd2Attempt.IsPassed)
                {
                    return Result.Failure(new Error("BusinessRule.PreDefense", "Work already passed Pre-Defense 2. No need for Pre-Defense 3."));
                }
                break;

            case Domain.Wf.Entities.WorkStates.Cancelled:
                // If cancelled from PD3, it means failed final chance
                var pd3Attempt = attempts.FirstOrDefault(a => a.PreDefenseNumber == 3);
                if (pd3Attempt != null && pd3Attempt.IsPassed)
                {
                    return Result.Failure(new Error("BusinessRule.PreDefense", "Work passed Pre-Defense 3 and cannot be cancelled for failure now."));
                }
                break;
        }

        // Business rule: Before entering Anti-plagiarism stage, verify all mandatory "initial" checks (like NormControl) are passed
        if (targetState.SystemName == Domain.Wf.Entities.WorkStates.ChecksWaitingForAntiPlagiarism)
        {
            var firstParticipant = work.Participants.FirstOrDefault();
            if (firstParticipant is null)
                return Result.Failure(new Error("BusinessRule.Transition", "Work has no participants."));

            var student = await _studentRepository.GetByIdAsync(firstParticipant.StudentId, cancellationToken);
            if (student is null)
                return Result.Failure(new Error("404", "Student not found."));

            var mandatoryChecks = student.SpecialityId.HasValue
                ? await _specialityCheckTypeRepository.GetBySpecialityAsync(student.SpecialityId.Value, cancellationToken)
                : Array.Empty<SpecialityCheckType>();
            
            // Initial checks are all mandatory checks except Anti-plagiarism
            var initialCheckTypeIds = mandatoryChecks
                .Where(mc => mc.CheckType?.Code != Domain.Thesis.Constants.CheckTypeCodes.AntiPlagiarism)
                .Select(c => c.CheckTypeId)
                .ToList();

            foreach (var checkTypeId in initialCheckTypeIds)
            {
                if (!work.HasPassedCheck(checkTypeId))
                {
                    var checkType = mandatoryChecks.First(mc => mc.CheckTypeId == checkTypeId).CheckType;
                    return Result.Failure(new Error("BusinessRule.InitialChecks", 
                        $"Cannot proceed to Anti-plagiarism until mandatory check '{checkType?.Title ?? checkTypeId.ToString()}' is passed."));
                }
            }
        }

        // Business rule: Before entering ReadyForDefense or Defended, verify eligibility
        if (targetState.SystemName == Domain.Wf.Entities.WorkStates.ReadyForDefense ||
            targetState.SystemName == Domain.Wf.Entities.WorkStates.Defended)
        {
            var firstParticipant = work.Participants.FirstOrDefault();
            if (firstParticipant is null)
                return Result.Failure(new Error("BusinessRule.Transition", "Work has no participants."));

            var student = await _studentRepository.GetByIdAsync(firstParticipant.StudentId, cancellationToken);
            if (student is null)
                return Result.Failure(new Error("404", "Student not found."));

            var mandatoryChecks = student.SpecialityId.HasValue
                ? await _specialityCheckTypeRepository.GetBySpecialityAsync(student.SpecialityId.Value, cancellationToken)
                : Array.Empty<SpecialityCheckType>();
            var mandatoryCheckTypeIds = mandatoryChecks.Select(c => c.CheckTypeId).ToList();

            if (!work.IsEligibleForDefense(mandatoryCheckTypeIds))
                return Result.Failure(new Error("BusinessRule.DefenseEligibility", "Not all mandatory quality checks have been passed for this speciality."));
        }

        work.ChangeState(request.TargetStateId, userId, request.Comment);
        await _studentWorkRepository.UpdateAsync(work, cancellationToken);
        return Result.Success();
    }
}

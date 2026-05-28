using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Checks.Commands.CompleteQualityCheck;

public record CompleteQualityCheckCommand(
    long WorkId,
    long CheckId,
    bool IsPassed,
    decimal? ResultValue,
    string? Comment,
    long? AttachmentId) : IRequest<Result<Unit>>;

public sealed class CompleteQualityCheckCommandHandler : IRequestHandler<CompleteQualityCheckCommand, Result<Unit>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly ISpecialityCheckTypeRepository _specialityCheckTypeRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteQualityCheckCommandHandler(
        IStudentWorkRepository studentWorkRepository,
        ICurrentUserProvider currentUserProvider,
        IStaffAssignmentRepository staffAssignmentRepository,
        ISpecialityCheckTypeRepository specialityCheckTypeRepository,
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork)
    {
        _studentWorkRepository = studentWorkRepository;
        _currentUserProvider = currentUserProvider;
        _staffAssignmentRepository = staffAssignmentRepository;
        _specialityCheckTypeRepository = specialityCheckTypeRepository;
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(CompleteQualityCheckCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<Unit>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure<Unit>(new Error("StudentWorks.NotFound", $"Student work with ID {request.WorkId} not found."));
        }

        var check = work.QualityChecks.FirstOrDefault(c => c.Id == request.CheckId);
        if (check == null)
        {
            return Result.Failure<Unit>(new Error("QualityChecks.NotFound", $"Quality check with ID {request.CheckId} not found on this work."));
        }

        // Verify that the expert has an active assignment for this check type
        var userAssignments = await _staffAssignmentRepository.GetByUserAsync(currentUserId, cancellationToken);
        var hasAccess = userAssignments.Any(a =>
            a.IsActive && !a.IsDeleted &&
            a.TargetEntityType == "OrgUnit" &&
            a.TargetEntityId == work.OrgUnitId &&
            a.RoleType == StaffRoleType.QualityExpert &&
            HasExpertCheckTypeAccess(a.MetadataJson, check.CheckTypeId));

        if (!hasAccess)
        {
            return Result.Failure<Unit>(new Error("Checks.Forbidden", "You do not have permission to evaluate this check type."));
        }

        // Determine final isPassed based on minimum pass value for AntiPlagiarism (ID = 2) or other check types
        bool finalIsPassed = request.IsPassed;
        if (check.CheckTypeId == 2 && request.ResultValue.HasValue)
        {
            // Load configuration
            var configs = await _specialityCheckTypeRepository.GetByOrgUnitAsync(work.OrgUnitId, cancellationToken);
            var activeConfig = configs
                .Where(c => c.IsActive && c.CheckTypeId == 2)
                .OrderByDescending(c => c.SpecialityId.HasValue) // Priority: Specific speciality first
                .FirstOrDefault(c => !c.SpecialityId.HasValue || c.SpecialityId.Value == work.SpecialityId);

            if (activeConfig != null && activeConfig.MinimumPassValue.HasValue)
            {
                if (request.ResultValue.Value < activeConfig.MinimumPassValue.Value)
                {
                    finalIsPassed = false; // Override isPassed to false if below threshold
                }
            }
        }

        // Complete the quality check
        work.CompleteQualityCheck(request.CheckId, currentUserId, finalIsPassed, request.ResultValue, request.Comment, request.AttachmentId);

        // State Machine Transition Logic
        if (finalIsPassed)
        {
            var currentState = await _workflowRepository.GetStateByIdAsync(work.CurrentStateId, cancellationToken);
            if (currentState != null)
            {
                int workTypeId = currentState.WorkTypeId;

                // Load all active configurations for this department
                var configs = await _specialityCheckTypeRepository.GetByOrgUnitAsync(work.OrgUnitId, cancellationToken);
                
                // Select active configurations for the work's speciality, fallback to department default
                var activeConfigs = configs.Where(c => c.IsActive).ToList();
                var specialityConfigs = activeConfigs.Where(c => c.SpecialityId.HasValue && c.SpecialityId.Value == work.SpecialityId).ToList();
                var selectedConfigs = specialityConfigs.Any() 
                    ? specialityConfigs 
                    : activeConfigs.Where(c => !c.SpecialityId.HasValue).ToList();

                var passedCheckTypeIds = work.QualityChecks.Where(c => c.IsPassed).Select(c => c.CheckTypeId).ToHashSet();

                if (check.CheckTypeId == 2) // AntiPlagiarism
                {
                    // Transition to ReviewsWaitingForSupervisor
                    var waitingForSupervisorState = await _workflowRepository.GetStateBySystemNameAsync(workTypeId, WorkStates.ReviewsWaitingForSupervisor, cancellationToken);
                    if (waitingForSupervisorState == null)
                        return Result.Failure<Unit>(new Error("Workflow.StateNotFound", $"Target state '{WorkStates.ReviewsWaitingForSupervisor}' not found for work type {workTypeId}."));

                    work.ChangeState(waitingForSupervisorState.Id, currentUserId, "Антиплагиат успешно пройден. Ожидание отзыва руководителя.");
                }
                else // NormControl or SoftwareCheck, etc. (Initial checks)
                {
                    // Find all required initial checks (CheckTypeId != 2)
                    var requiredInitialCheckTypeIds = selectedConfigs
                        .Where(c => c.CheckTypeId != 2)
                        .Select(c => c.CheckTypeId)
                        .ToList();

                    // Fallback to default: NormControl (ID = 1) is always required if no configs defined
                    if (!requiredInitialCheckTypeIds.Any())
                    {
                        requiredInitialCheckTypeIds.Add(1);
                    }

                    // Check if all required initial checks are passed
                    bool allInitialPassed = requiredInitialCheckTypeIds.All(id => passedCheckTypeIds.Contains(id));
                    if (allInitialPassed)
                    {
                        var antiPlagiarismState = await _workflowRepository.GetStateBySystemNameAsync(workTypeId, WorkStates.ChecksWaitingForAntiPlagiarism, cancellationToken);
                        if (antiPlagiarismState == null)
                            return Result.Failure<Unit>(new Error("Workflow.StateNotFound", $"Target state '{WorkStates.ChecksWaitingForAntiPlagiarism}' not found for work type {workTypeId}."));

                        work.ChangeState(antiPlagiarismState.Id, currentUserId, "Начальные проверки успешно пройдены. Ожидание антиплагиата.");
                    }
                }
            }
        }

        await _studentWorkRepository.UpdateAsync(work, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }

    private static bool HasExpertCheckTypeAccess(string? metadataJson, int checkTypeId)
    {
        if (string.IsNullOrEmpty(metadataJson)) return false;
        try
        {
            using var doc = JsonDocument.Parse(metadataJson);
            if (doc.RootElement.TryGetProperty("CheckTypeId", out var prop))
            {
                if (prop.ValueKind == JsonValueKind.Number)
                {
                    return prop.GetInt32() == checkTypeId;
                }
            }
        }
        catch { }
        return false;
    }
}

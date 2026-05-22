namespace AWM.Service.Application.Features.Thesis.Directions.Commands.CreateDirection;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for creating a new research direction.
/// </summary>
public sealed class CreateDirectionCommandHandler
    : IRequestHandler<CreateDirectionCommand, Result<long>>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IOrganizationLookupRepository _organizationLookupRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IStageValidationService _stageValidationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateDirectionCommandHandler> _logger;

    public CreateDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        IOrganizationLookupRepository organizationLookupRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        ICurrentUserProvider currentUserProvider,
        IStageValidationService stageValidationService,
        IUnitOfWork unitOfWork,
        ILogger<CreateDirectionCommandHandler> logger)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _organizationLookupRepository = organizationLookupRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
        _currentUserProvider = currentUserProvider;
        _stageValidationService = stageValidationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<long>> Handle(
        CreateDirectionCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId;
        if (!currentUserId.HasValue)
        {
            return Result.Failure<long>(new Error("401", "User ID is not available."));
        }

        // Supervisor ID is now UserId. If not provided, use current user.
        var targetSupervisorUserId = request.SupervisorId > 0 ? request.SupervisorId : currentUserId.Value;

        // Validate department exists
        var department = await _organizationLookupRepository
            .GetDepartmentByIdAsync(request.DepartmentId, cancellationToken);

        if (department is null)
        {
            return Result.Failure<long>(new Error("404", $"Department with ID {request.DepartmentId} not found."));
        }

        // Validate that DirectionSubmission stage is open
        var (isAllowed, errorMessage) = await _stageValidationService
            .ValidateOperationInStageAsync(request.DepartmentId, request.AcademicYearId, 1, null, cancellationToken);

        if (!isAllowed)
        {
            return Result.Failure<long>(new Error("409", errorMessage!));
        }

        // Validate work type exists
        var workType = await _workflowRepository.GetWorkTypeByIdAsync(request.WorkTypeId, cancellationToken);
        if (workType is null)
        {
            return Result.Failure<long>(new Error("404", $"Work type with ID {request.WorkTypeId} not found."));
        }

        // Get initial "Draft" state
        var draftState = await _workflowRepository.GetStateBySystemNameAsync(request.WorkTypeId, DirectionStates.Draft, cancellationToken);
        if (draftState is null)
        {
            return Result.Failure<long>(new Error("404", $"Draft state not found for work type {request.WorkTypeId}."));
        }

        try
        {
            // Create direction entity
            var direction = new Direction(
                orgUnitId: request.DepartmentId,
                createdByUserId: currentUserId.Value,
                semesterId: request.AcademicYearId,
                workTypeId: request.WorkTypeId,
                titleRu: request.TitleRu,
                draftStateId: draftState.Id,
                titleKz: request.TitleKz,
                titleEn: request.TitleEn,
                descriptionRu: request.DescriptionRu,
                descriptionKz: request.DescriptionKz,
                descriptionEn: request.DescriptionEn);

            await _directionRepository.AddAsync(direction, cancellationToken);
            
            // Critical: Save changes first to get the direction.Id if it's identity
            // However, Direction uses long and might be identity. 
            // In AWM, we usually use SaveChanges to get ID.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Create StaffAssignment for the supervisor
            var assignment = new StaffAssignment(
                userId: targetSupervisorUserId,
                roleType: StaffRoleType.Supervisor,
                targetEntityType: "Direction",
                targetEntityId: direction.Id,
                createdBy: currentUserId.Value);

            await _staffAssignmentRepository.AddAsync(assignment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(direction.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateDirection failed");
            return Result.Failure<long>(new Error("500", ex.Message));
        }
    }
}

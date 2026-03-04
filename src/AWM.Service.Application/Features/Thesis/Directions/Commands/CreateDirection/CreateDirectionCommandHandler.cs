namespace AWM.Service.Application.Features.Thesis.Directions.Commands.CreateDirection;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
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
    private readonly IStaffRepository _staffRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IPeriodValidationService _periodValidationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateDirectionCommandHandler> _logger;

    public CreateDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        IOrganizationLookupRepository organizationLookupRepository,
        IStaffRepository staffRepository,
        ICurrentUserProvider currentUserProvider,
        IPeriodValidationService periodValidationService,
        IUnitOfWork unitOfWork,
        ILogger<CreateDirectionCommandHandler> logger)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _organizationLookupRepository = organizationLookupRepository;
        _staffRepository = staffRepository;
        _currentUserProvider = currentUserProvider;
        _periodValidationService = periodValidationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<long>> Handle(
        CreateDirectionCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to create Direction in Dept={DeptId} by CurrentUserId={CurrentUserId}",
            request.DepartmentId, currentUserId);

        if (!currentUserId.HasValue)
        {
            _logger.LogWarning("CreateDirection failed: Current user ID is not available.");
            return Result.Failure<long>(new Error("401", "User ID is not available."));
        }

        // Validate department exists
        var department = await _organizationLookupRepository
            .GetDepartmentByIdAsync(request.DepartmentId, cancellationToken);

        if (department is null)
        {
            _logger.LogWarning("CreateDirection failed: Department {DeptId} not found.", request.DepartmentId);
            return Result.Failure<long>(new Error(
                "404",
                $"Department with ID {request.DepartmentId} not found."));
        }

        // Validate that DirectionSubmission period is open
        var (isAllowed, errorMessage) = await _periodValidationService
            .ValidateOperationInPeriodAsync(request.DepartmentId, request.AcademicYearId,
                WorkflowStage.DirectionSubmission, cancellationToken);

        if (!isAllowed)
        {
            _logger.LogWarning("CreateDirection failed: Period closed - {Error}", errorMessage);
            return Result.Failure<long>(new Error("409", errorMessage!));
        }

        // Validate work type exists
        var workType = await _workflowRepository
            .GetWorkTypeByIdAsync(request.WorkTypeId, cancellationToken);

        if (workType is null)
        {
            _logger.LogWarning("CreateDirection failed: WorkType {WorkTypeId} not found.", request.WorkTypeId);
            return Result.Failure<long>(new Error(
                "404",
                $"Work type with ID {request.WorkTypeId} not found."));
        }

        // Get initial "Draft" state for this work type
        var draftState = await _workflowRepository
            .GetStateBySystemNameAsync(request.WorkTypeId, "Draft", cancellationToken);

        if (draftState is null)
        {
            _logger.LogError("CreateDirection failed: Draft state not found for work type {WorkTypeId}.", request.WorkTypeId);
            return Result.Failure<long>(new Error(
                "404",
                $"Draft state not found for work type {request.WorkTypeId}."));
        }

        try
        {
            var supervisorId = request.SupervisorId;
            if (supervisorId <= 0)
            {
                var staff = await _staffRepository.GetByUserIdAsync(currentUserId.Value, cancellationToken);
                if (staff is null)
                {
                    return Result.Failure<long>(new Error("403", "User does not have an associated staff profile to act as a supervisor."));
                }
                supervisorId = staff.Id;
            }

            _logger.LogDebug("Determined SupervisorId: {SupervisorId} (Requested: {RequestedId}, CurrentUser: {CurrentUserId})",
                supervisorId, request.SupervisorId, currentUserId.Value);

            // Create direction entity using domain constructor
            var direction = new Direction(
                departmentId: request.DepartmentId,
                supervisorId: supervisorId,
                academicYearId: request.AcademicYearId,
                workTypeId: request.WorkTypeId,
                titleRu: request.TitleRu,
                draftStateId: draftState.Id,
                titleKz: request.TitleKz,
                titleEn: request.TitleEn,
                description: request.Description);

            // Save to repository
            await _directionRepository.AddAsync(direction, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully created Direction {DirectionId} by user {CurrentUserId}", direction.Id, currentUserId.Value);
            return Result.Success(direction.Id);
        }
        catch (ArgumentException ex)
        {
            // Domain validation errors (from entity constructor)
            _logger.LogWarning(ex, "CreateDirection failed: Domain validation error - {Message}", ex.Message);
            return Result.Failure<long>(new Error("400", ex.Message));
        }
        catch (Exception ex)
        {
            // Unexpected errors
            _logger.LogError(ex, "CreateDirection failed: Unexpected error");
            return Result.Failure<long>(new Error("500", ex.Message));
        }
    }
}
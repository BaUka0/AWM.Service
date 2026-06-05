using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Constants;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Directions.Commands.CreateDirection;

/// <summary>
/// Command handler for creating a new thesis direction.
/// </summary>
public sealed class CreateDirectionCommandHandler : IRequestHandler<CreateDirectionCommand, Result<long>>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IEmployeeReadOnlyRepository _employeeRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IStageValidationService _stageValidationService;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDirectionCommandHandler"/> class.
    /// </summary>
    public CreateDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        IEmployeeReadOnlyRepository employeeRepository,
        ICurrentUserProvider currentUserProvider,
        IStageValidationService stageValidationService,
        IUnitOfWork unitOfWork)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _employeeRepository = employeeRepository;
        _currentUserProvider = currentUserProvider;
        _stageValidationService = stageValidationService;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the request to create a direction.
    /// </summary>
    public async Task<Result<long>> Handle(CreateDirectionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<long>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        // Resolve OrgUnitId
        int orgUnitId;
        if (request.OrgUnitId.HasValue)
        {
            orgUnitId = request.OrgUnitId.Value;
        }
        else
        {
            var employee = await _employeeRepository.GetByUserIdAsync(currentUserId, cancellationToken);
            if (employee == null)
            {
                return Result.Failure<long>(new Error("Directions.EmployeeNotFound", "Employee record not found for the current user in University SoT."));
            }

            var mainPosition = employee.Positions.FirstOrDefault(p => p.IsMainPosition)
                                ?? employee.Positions.FirstOrDefault();

            if (mainPosition == null)
            {
                return Result.Failure<long>(new Error("Directions.OrgUnitNotFound", "Employee has no assigned department in University SoT."));
            }

            orgUnitId = mainPosition.OrgUnitId;
        }

        // Validate that the DirectionProposal stage is open
        var (isAllowed, errorMessage) = await _stageValidationService.ValidateOperationInStageAsync(
            orgUnitId,
            request.SemesterId,
            WorkflowStageIds.DirectionProposal,
            cancellationToken: cancellationToken);

        if (!isAllowed)
        {
            return Result.Failure<long>(new Error("Directions.StageClosed", errorMessage ?? "The direction formation stage is closed."));
        }

        var draftState = await _workflowRepository.GetStateBySystemNameAsync(request.WorkTypeId, DirectionStates.Draft, cancellationToken);

        if (draftState == null)
            return Result.Failure<long>(new Error("State.NotFound", "Draft state not found in the system."));

        var direction = new Direction(
            orgUnitId: orgUnitId,
            createdByUserId: currentUserId,
            semesterId: request.SemesterId,
            workTypeId: request.WorkTypeId,
            titleRu: request.TitleRu,
            draftStateId: draftState.Id,
            titleKz: request.TitleKz,
            titleEn: request.TitleEn,
            descriptionRu: request.DescriptionRu,
            descriptionKz: request.DescriptionKz,
            descriptionEn: request.DescriptionEn);

        await _directionRepository.AddAsync(direction, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(direction.Id);
    }
}

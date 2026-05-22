namespace AWM.Service.Application.Features.Thesis.Topics.Commands.CreateTopic;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for creating a new thesis topic.
/// </summary>
public sealed class CreateTopicCommandHandler : IRequestHandler<CreateTopicCommand, Result<long>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IStageValidationService _stageValidationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ILogger<CreateTopicCommandHandler> _logger;

    public CreateTopicCommandHandler(
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        IStageValidationService stageValidationService,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider,
        ILogger<CreateTopicCommandHandler> logger)
    {
        _topicRepository = topicRepository;
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
        _stageValidationService = stageValidationService;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
        _logger = logger;
    }

    public async Task<Result<long>> Handle(CreateTopicCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId;
        if (!currentUserId.HasValue)
        {
            return Result.Failure<long>(new Error("401", "User ID is not available."));
        }

        // Supervisor ID is now UserId. If not provided, use current user.
        var targetSupervisorUserId = request.SupervisorId > 0 ? request.SupervisorId : currentUserId.Value;

        try
        {
            // Validate that TopicCreation stage is open
            var (isAllowed, errorMessage) = await _stageValidationService
                .ValidateOperationInStageAsync(request.DepartmentId, request.AcademicYearId, 2, null, cancellationToken);

            if (!isAllowed)
            {
                return Result.Failure<long>(new Error("409", errorMessage!));
            }

            // Verify Direction exists and is approved
            if (request.DirectionId.HasValue)
            {
                var direction = await _directionRepository.GetByIdAsync(request.DirectionId.Value, cancellationToken);
                if (direction is null)
                {
                    return Result.Failure<long>(new Error("404", $"Direction with ID {request.DirectionId} not found."));
                }

                var approvedState = await _workflowRepository.GetStateBySystemNameAsync(direction.WorkTypeId, DirectionStates.Approved, cancellationToken);
                if (approvedState == null || direction.CurrentStateId != approvedState.Id)
                {
                    return Result.Failure<long>(new Error("BusinessRule.Direction", "Topics can only be created for approved research directions."));
                }
            }

            // Create topic using domain constructor
            var topic = new Topic(
                orgUnitId: request.DepartmentId,
                createdByUserId: currentUserId.Value,
                semesterId: request.AcademicYearId,
                workTypeId: request.WorkTypeId,
                titleRu: request.TitleRu,
                directionId: request.DirectionId,
                titleKz: request.TitleKz,
                titleEn: request.TitleEn,
                descriptionRu: request.DescriptionRu,
                descriptionKz: request.DescriptionKz,
                descriptionEn: request.DescriptionEn,
                maxParticipants: request.MaxParticipants,
                specialityId: request.SpecialityId);

            await _topicRepository.AddAsync(topic, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Create StaffAssignment for the supervisor
            var assignment = new StaffAssignment(
                userId: targetSupervisorUserId,
                roleType: StaffRoleType.Supervisor,
                targetEntityType: "Topic",
                targetEntityId: topic.Id,
                createdBy: currentUserId.Value);

            await _staffAssignmentRepository.AddAsync(assignment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(topic.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateTopic failed");
            return Result.Failure<long>(new Error("500", ex.Message));
        }
    }
}

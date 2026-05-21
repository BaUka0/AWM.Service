namespace AWM.Service.Application.Features.Thesis.Topics.Commands.CreateTopic;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
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
    private readonly IEmployeeRepository _EmployeeRepository;
    private readonly IStageValidationService _stageValidationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ILogger<CreateTopicCommandHandler> _logger;

    public CreateTopicCommandHandler(
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        IEmployeeRepository EmployeeRepository,
        IStageValidationService stageValidationService,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider,
        ILogger<CreateTopicCommandHandler> logger)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
        _EmployeeRepository = EmployeeRepository ?? throw new ArgumentNullException(nameof(EmployeeRepository));
        _stageValidationService = stageValidationService ?? throw new ArgumentNullException(nameof(stageValidationService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<long>> Handle(CreateTopicCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to create Topic in Dept={DeptId} by CurrentUserId={CurrentUserId}",
            request.DepartmentId, currentUserId);

        try
        {
            if (!currentUserId.HasValue)
            {
                _logger.LogWarning("CreateTopic failed: Current user ID is not available.");
                return Result.Failure<long>(new Error("401", "User ID is not available."));
            }

            // Validate that TopicCreation stage is open
            var (isAllowed, errorMessage) = await _stageValidationService
                .ValidateOperationInStageAsync(request.DepartmentId, request.AcademicYearId,
                    2, cancellationToken);

            if (!isAllowed)
            {
                _logger.LogWarning("CreateTopic failed: Period closed - {Error}", errorMessage);
                return Result.Failure<long>(new Error("409", errorMessage!));
            }

            // 2. If DirectionId is provided, verify it exists and is approved
            if (request.DirectionId.HasValue)
            {
                var direction = await _directionRepository.GetByIdAsync(request.DirectionId.Value, cancellationToken);

                if (direction is null)
                {
                    _logger.LogWarning("CreateTopic failed: Direction {DirectionId} not found.", request.DirectionId.Value);
                    return Result.Failure<long>(new Error("404", $"Direction with ID {request.DirectionId} not found."));
                }

                // Business rule: Topics can only be created for approved directions
                var approvedState = await _workflowRepository.GetStateBySystemNameAsync(direction.WorkTypeId, DirectionStates.Approved, cancellationToken);
                if (approvedState == null || direction.CurrentStateId != approvedState.Id)
                {
                    _logger.LogWarning("CreateTopic failed: Direction {DirectionId} is not approved.", request.DirectionId.Value);
                    return Result.Failure<long>(new Error("BusinessRule.Direction", "Topics can only be created for approved research directions."));
                }
            }

            var supervisorId = request.SupervisorId;
            if (supervisorId <= 0)
            {
                var staff = await _EmployeeRepository.GetByUserIdAsync(currentUserId.Value, cancellationToken);
                if (staff is null)
                {
                    return Result.Failure<long>(new Error("403", "User does not have an associated staff profile to act as a supervisor."));
                }
                supervisorId = staff.Id;
            }
            _logger.LogDebug("Determined SupervisorId: {SupervisorId} (Requested: {RequestedId}, CurrentUser: {CurrentUserId})",
                supervisorId, request.SupervisorId, currentUserId.Value);

            // 3. Create topic using domain constructor
            var topic = new Topic(
                orgUnitId: request.DepartmentId,
                employeeId: supervisorId,
                semesterId: request.AcademicYearId,
                workTypeId: request.WorkTypeId,
                titleRu: request.TitleRu,
                directionId: request.DirectionId,
                titleKz: request.TitleKz,
                titleEn: request.TitleEn,
                descriptionRu: request.DescriptionRu,
                descriptionKz: request.DescriptionKz,
                descriptionEn: request.DescriptionEn,
                maxParticipants: request.MaxParticipants);

            // 4. Add to repository
            await _topicRepository.AddAsync(topic, cancellationToken);

            // 5. Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully created Topic {TopicId} by user {CurrentUserId}", topic.Id, currentUserId.Value);
            return Result.Success(topic.Id);
        }
        catch (ArgumentException argEx)
        {
            // Domain validation errors
            _logger.LogWarning(argEx, "CreateTopic failed: Domain validation error - {Message}", argEx.Message);
            return Result.Failure<long>(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            // Unexpected errors
            _logger.LogError(ex, "CreateTopic failed: Unexpected error");
            return Result.Failure<long>(new Error("500", ex.Message));
        }
    }
}

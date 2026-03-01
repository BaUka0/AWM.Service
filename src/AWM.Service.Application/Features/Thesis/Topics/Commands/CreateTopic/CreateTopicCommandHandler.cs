namespace AWM.Service.Application.Features.Thesis.Topics.Commands.CreateTopic;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ILogger<CreateTopicCommandHandler> _logger;

    public CreateTopicCommandHandler(
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider,
        ILogger<CreateTopicCommandHandler> logger)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
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
                // Assuming CurrentStateId represents workflow state (we'd need to check if it's "Approved" state)
                // For now, we'll skip this checkor you can add state validation if you have state IDs
            }

            var supervisorId = request.SupervisorId > 0 ? request.SupervisorId : currentUserId.Value;
            _logger.LogDebug("Determined SupervisorId: {SupervisorId} (Requested: {RequestedId}, CurrentUser: {CurrentUserId})",
                supervisorId, request.SupervisorId, currentUserId.Value);

            // 3. Create topic using domain constructor
            var topic = new Topic(
                departmentId: request.DepartmentId,
                supervisorId: supervisorId,
                academicYearId: request.AcademicYearId,
                workTypeId: request.WorkTypeId,
                titleRu: request.TitleRu,
                directionId: request.DirectionId,
                titleKz: request.TitleKz,
                titleEn: request.TitleEn,
                description: request.Description,
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
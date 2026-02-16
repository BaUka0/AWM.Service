namespace AWM.Service.Application.Features.Thesis.Topics.Commands.CreateTopic;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for creating a new thesis topic.
/// </summary>
public sealed class CreateTopicCommandHandler : IRequestHandler<CreateTopicCommand, Result<long>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateTopicCommandHandler(
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _directionRepository = directionRepository ?? throw new ArgumentNullException(nameof(directionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<long>> Handle(CreateTopicCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
            {
                return Result.Failure<long>(new Error("401", "User ID is not available."));
            }

            // 2. If DirectionId is provided, verify it exists and is approved
            if (request.DirectionId.HasValue)
            {
                var direction = await _directionRepository.GetByIdAsync(request.DirectionId.Value, cancellationToken);

                if (direction is null)
                {
                    return Result.Failure<long>(new Error("404", $"Direction with ID {request.DirectionId} not found."));
                }

                // Business rule: Topics can only be created for approved directions
                // Assuming CurrentStateId represents workflow state (we'd need to check if it's "Approved" state)
                // For now, we'll skip this checkor you can add state validation if you have state IDs
            }

            // 3. Create topic using domain constructor
            var topic = new Topic(
                departmentId: request.DepartmentId,
                supervisorId: userId.Value, // Use current user ID as supervisor ID
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

            return Result.Success(topic.Id);
        }
        catch (ArgumentException argEx)
        {
            // Domain validation errors
            return Result.Failure<long>(new Error("400", argEx.Message));
        }
        catch (Exception ex)
        {
            // Unexpected errors
            return Result.Failure<long>(new Error("500", ex.Message));
        }
    }
}
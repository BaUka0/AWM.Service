using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Constants;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.CreateTopic;

public sealed class CreateTopicCommandHandler : IRequestHandler<CreateTopicCommand, Result<long>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IEmployeeReadOnlyRepository _employeeRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IStageValidationService _stageValidationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTopicCommandHandler(
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        IEmployeeReadOnlyRepository employeeRepository,
        ICurrentUserProvider currentUserProvider,
        IStageValidationService stageValidationService,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository;
        _directionRepository = directionRepository;
        _employeeRepository = employeeRepository;
        _currentUserProvider = currentUserProvider;
        _stageValidationService = stageValidationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<long>> Handle(CreateTopicCommand request, CancellationToken cancellationToken)
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
                return Result.Failure<long>(new Error("Topics.EmployeeNotFound", "Employee record not found for the current user."));
            }

            var mainPosition = employee.Positions.FirstOrDefault(p => p.IsMainPosition) 
                                ?? employee.Positions.FirstOrDefault();
            
            if (mainPosition == null)
            {
                return Result.Failure<long>(new Error("Topics.OrgUnitNotFound", "Employee has no assigned department."));
            }

            orgUnitId = mainPosition.OrgUnitId;
        }

        // Validate Direction if provided
        if (request.DirectionId.HasValue)
        {
            var direction = await _directionRepository.GetByIdAsync(request.DirectionId.Value, cancellationToken);
            if (direction == null)
            {
                return Result.Failure<long>(new Error("Topics.DirectionNotFound", "Specified direction not found."));
            }
            
            // Note: In a real system, we'd check if the direction is Approved.
            // But here we'll proceed as per domain entity constraints.
        }

        // Validate that the TopicProposal stage (Stage 4) is open
        var (isAllowed, errorMessage) = await _stageValidationService.ValidateOperationInStageAsync(
            orgUnitId,
            request.SemesterId,
            WorkflowStageIds.TopicProposal,
            cancellationToken: cancellationToken);

        if (!isAllowed)
        {
            return Result.Failure<long>(new Error("Topics.StageClosed", errorMessage ?? "The topic formation stage is closed."));
        }

        var topic = new Topic(
            orgUnitId: orgUnitId,
            createdByUserId: currentUserId,
            semesterId: request.SemesterId,
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

        return Result.Success(topic.Id);
    }
}

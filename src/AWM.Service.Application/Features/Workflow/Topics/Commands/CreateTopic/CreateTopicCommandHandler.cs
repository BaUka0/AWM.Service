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
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IOrgUnitResolver _orgUnitResolver;
    private readonly IStageValidationService _stageValidationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTopicCommandHandler(
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository,
        ICurrentUserProvider currentUserProvider,
        IOrgUnitResolver orgUnitResolver,
        IStageValidationService stageValidationService,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository;
        _directionRepository = directionRepository;
        _currentUserProvider = currentUserProvider;
        _orgUnitResolver = orgUnitResolver;
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

        // Resolve OrgUnitId via universal resolver (supports Employee + Student)
        var (resolvedOrgUnitId, orgUnitError) = await _orgUnitResolver.ResolveAsync(request.OrgUnitId, currentUserId, cancellationToken);
        if (!resolvedOrgUnitId.HasValue)
        {
            return Result.Failure<long>(new Error("OrgUnit.CannotResolve", orgUnitError ?? "Unable to determine department."));
        }

        var orgUnitId = resolvedOrgUnitId.Value;

        // Validate Direction if provided
        if (request.DirectionId.HasValue)
        {
            var direction = await _directionRepository.GetByIdAsync(request.DirectionId.Value, cancellationToken);
            if (direction == null)
            {
                return Result.Failure<long>(new Error("Topics.DirectionNotFound", "Specified direction not found."));
            }
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

        // Raise domain event after SaveChanges so Id is assigned by DB
        topic.RaiseCreatedEvent();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(topic.Id);
    }
}

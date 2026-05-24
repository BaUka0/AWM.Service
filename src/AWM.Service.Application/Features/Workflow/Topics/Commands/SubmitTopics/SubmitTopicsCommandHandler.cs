using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Constants;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.SubmitTopics;

public sealed class SubmitTopicsCommandHandler : IRequestHandler<SubmitTopicsCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IStageValidationService _stageValidationService;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitTopicsCommandHandler(
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IStageValidationService stageValidationService,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _stageValidationService = stageValidationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SubmitTopicsCommand request, CancellationToken cancellationToken)
    {
        if (request.TopicIds == null || !request.TopicIds.Any())
            return Result.Failure(new Error("Topics.EmptyList", "No topic IDs provided."));

        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;
        var topics = await _topicRepository.GetByIdsAsync(request.TopicIds, cancellationToken);

        if (topics.Count != request.TopicIds.Count)
            return Result.Failure(new Error("Topics.NotFound", "Some topics were not found."));

        foreach (var topic in topics)
        {
            if (topic.CreatedBy != currentUserId)
                return Result.Failure(new Error("Topics.Unauthorized", $"You are not authorized to submit topic ID {topic.Id}."));

            if (topic.IsApproved)
                continue; // Already approved topics can be skipped or throw error depending on preference

            // Validate stage for each topic (though they should share the same orgUnit/semester)
            var (isAllowed, errorMessage) = await _stageValidationService.ValidateOperationInStageAsync(
                topic.OrgUnitId,
                topic.SemesterId,
                WorkflowStageIds.TopicProposal,
                cancellationToken: cancellationToken);

            if (!isAllowed)
                return Result.Failure(new Error("Topics.StageClosed", $"Stage 4 is closed for topic ID {topic.Id}. {errorMessage}"));

            topic.SubmitForApproval();
            await _topicRepository.UpdateAsync(topic, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.UpdateTopic;

public sealed class UpdateTopicCommandHandler : IRequestHandler<UpdateTopicCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTopicCommandHandler(
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateTopicCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var topic = await _topicRepository.GetByIdAsync(request.Id, cancellationToken);
        if (topic == null)
            return Result.Failure(new Error("Topics.NotFound", "Topic not found."));

        if (topic.CreatedBy != _currentUserProvider.UserId.Value)
            return Result.Failure(new Error("Topics.Unauthorized", "You can only update your own topics."));

        if (topic.IsApproved)
            return Result.Failure(new Error("Topics.AlreadyApproved", "Approved topics cannot be updated."));

        topic.UpdateContent(
            request.TitleRu,
            request.TitleKz,
            request.TitleEn,
            request.DescriptionRu,
            request.DescriptionKz,
            request.DescriptionEn,
            request.MaxParticipants);

        await _topicRepository.UpdateAsync(topic, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

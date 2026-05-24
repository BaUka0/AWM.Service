using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Commands.RejectApplication;

public sealed class RejectApplicationCommandHandler : IRequestHandler<RejectApplicationCommand, Result>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RejectApplicationCommandHandler(
        ITopicApplicationRepository applicationRepository,
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _applicationRepository = applicationRepository;
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RejectApplicationCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;

        var application = await _applicationRepository.GetByIdWithTopicAsync(request.ApplicationId, cancellationToken);
        if (application == null)
            return Result.Failure(new Error("Applications.NotFound", "Application not found."));

        var topic = await _topicRepository.GetByIdAsync(application.TopicId, cancellationToken);
        if (topic == null)
            return Result.Failure(new Error("Topics.NotFound", "Topic not found."));

        // Validate that current user is the supervisor
        if (topic.CreatedBy != currentUserId)
            return Result.Failure(new Error("Applications.Unauthorized", "You are not authorized to reject applications for this topic."));

        // Reject application
        application.Reject(currentUserId, request.Reason);
        await _applicationRepository.UpdateAsync(application, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

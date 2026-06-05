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

        // Get application (detached, only for TopicId)
        var applicationInfo = await _applicationRepository.GetByIdAsync(request.ApplicationId, cancellationToken);
        if (applicationInfo == null)
            return Result.Failure(new Error("Applications.NotFound", "Application not found."));

        // Load topic with applications (tracked)
        var topic = await _topicRepository.GetByIdAsync(applicationInfo.TopicId, cancellationToken);
        if (topic == null)
            return Result.Failure(new Error("Topics.NotFound", "Topic not found."));

        var application = topic.Applications.FirstOrDefault(a => a.Id == request.ApplicationId);
        if (application == null)
            return Result.Failure(new Error("Applications.NotFound", "Application not found."));

        // Validate that current user is the supervisor
        if (topic.CreatedBy != currentUserId)
            return Result.Failure(new Error("Applications.Unauthorized", "You are not authorized to reject applications for this topic."));

        // Reject tracked application entity — no explicit Update needed
        application.Reject(currentUserId, request.Reason);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Workflow.Applications.Commands.AcceptApplication;

public sealed class AcceptApplicationCommandHandler : IRequestHandler<AcceptApplicationCommand, Result>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public AcceptApplicationCommandHandler(
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

    public async Task<Result> Handle(AcceptApplicationCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));

        var currentUserId = _currentUserProvider.UserId.Value;

        var applicationInfo = await _applicationRepository.GetByIdAsync(request.ApplicationId, cancellationToken);
        if (applicationInfo == null)
            return Result.Failure(new Error("Applications.NotFound", "Application not found."));

        var topic = await _topicRepository.GetByIdAsync(applicationInfo.TopicId, cancellationToken);
        if (topic == null)
            return Result.Failure(new Error("Topics.NotFound", "Topic not found."));

        var application = topic.Applications.FirstOrDefault(a => a.Id == request.ApplicationId);
        if (application == null)
            return Result.Failure(new Error("Applications.NotFound", "Application not found."));

        if (topic.CreatedBy != currentUserId)
            return Result.Failure(new Error("Applications.Unauthorized", "You are not authorized to accept applications for this topic."));

        if (!topic.CanAcceptApplications())
            return Result.Failure(new Error("Topics.Closed", "Topic has reached its participant limit or is closed."));

        application.Accept(currentUserId);

        var acceptedCount = topic.Applications.Count(a => a.StatusId == (int)ApplicationStatusType.Accepted);
        if (acceptedCount >= topic.MaxParticipants)
        {
            topic.Close();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

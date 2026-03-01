namespace AWM.Service.Application.Features.Thesis.Applications.Commands.RejectApplication;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Common;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for RejectApplicationCommand.
/// Allows supervisor to reject a student's application to their topic with a reason.
/// </summary>
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
        {
            return Result.Failure(new Error("Authorization.Unauthorized", "User identity could not be determined."));
        }

        var userId = _currentUserProvider.UserId.Value;

        // 1. Get application with topic (for authorization)
        var application = await _applicationRepository.GetByIdWithTopicAsync(
            request.ApplicationId,
            cancellationToken);

        if (application is null)
        {
            return Result.Failure(new Error("Application.NotFound", $"Application with ID {request.ApplicationId} not found."));
        }

        // 2. Check if application is deleted
        if (application.IsDeleted)
        {
            return Result.Failure(new Error("Application.Deleted", "Cannot reject a deleted application."));
        }

        // 3. Get the topic (we need it loaded separately for full checks)
        var topic = await _topicRepository.GetByIdAsync(application.TopicId, cancellationToken);
        if (topic is null)
        {
            return Result.Failure(new Error("Topic.NotFound", "Related topic not found."));
        }

        // 4. Check authorization - only the topic's supervisor can reject
        if (topic.SupervisorId != userId)
        {
            return Result.Failure(new Error("Authorization.Forbidden", "Only the topic supervisor can reject applications."));
        }

        // 5. Check if topic is deleted (optional check, less critical than for Accept)
        if (topic.IsDeleted)
        {
            return Result.Failure(new Error("Topic.Deleted", "Cannot reject applications for a deleted topic."));
        }

        // 6. Reject the application (domain method)
        try
        {
            application.Reject(userId, request.RejectReason);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(new Error("Application.InvalidState", ex.Message));
        }

        // 7. Update application
        try
        {
            await _applicationRepository.UpdateAsync(application, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Database.Error", $"Failed to reject application: {ex.Message}"));
        }
    }
}
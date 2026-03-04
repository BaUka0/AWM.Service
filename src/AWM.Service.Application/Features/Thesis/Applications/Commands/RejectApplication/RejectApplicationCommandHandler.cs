namespace AWM.Service.Application.Features.Thesis.Applications.Commands.RejectApplication;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for RejectApplicationCommand.
/// Allows supervisor to reject a student's application to their topic with a reason.
/// </summary>
public sealed class RejectApplicationCommandHandler : IRequestHandler<RejectApplicationCommand, Result>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RejectApplicationCommandHandler> _logger;

    public RejectApplicationCommandHandler(
        ITopicApplicationRepository applicationRepository,
        ITopicRepository topicRepository,
        IStaffRepository staffRepository,
        ICurrentUserProvider currentUserProvider,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<RejectApplicationCommandHandler> logger)
    {
        _applicationRepository = applicationRepository;
        _topicRepository = topicRepository;
        _staffRepository = staffRepository;
        _currentUserProvider = currentUserProvider;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(RejectApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to reject application ID={ApplicationId} by User={UserId}", request.ApplicationId, userId);

        if (!userId.HasValue)
        {
            _logger.LogWarning("RejectApplication failed: User identity could not be determined.");
            return Result.Failure(new Error("Authorization.Unauthorized", "User identity could not be determined."));
        }

        var supervisorUserId = userId.Value;

        // Resolve current user's StaffId
        var currentStaff = await _staffRepository.GetByUserIdAsync(supervisorUserId, cancellationToken);
        if (currentStaff is null)
        {
            _logger.LogWarning("RejectApplication failed: User {UserId} does not have an associated staff profile.", supervisorUserId);
            return Result.Failure(new Error("Authorization.Forbidden", "User does not have an associated staff profile."));
        }

        // 1. Get application with topic (for authorization)
        var application = await _applicationRepository.GetByIdWithTopicAsync(
            request.ApplicationId,
            cancellationToken);

        if (application is null)
        {
            _logger.LogWarning("RejectApplication failed: Application ID={ApplicationId} not found.", request.ApplicationId);
            return Result.Failure(new Error("Application.NotFound", $"Application with ID {request.ApplicationId} not found."));
        }

        // 2. Check if application is deleted
        if (application.IsDeleted)
        {
            _logger.LogWarning("RejectApplication failed: Application ID={ApplicationId} is deleted.", request.ApplicationId);
            return Result.Failure(new Error("Application.Deleted", "Cannot reject a deleted application."));
        }

        // 3. Get the topic (we need it loaded separately for full checks)
        var topic = await _topicRepository.GetByIdAsync(application.TopicId, cancellationToken);
        if (topic is null)
        {
            _logger.LogWarning("RejectApplication failed: Related topic ID={TopicId} not found for Application ID={ApplicationId}.", application.TopicId, request.ApplicationId);
            return Result.Failure(new Error("Topic.NotFound", "Related topic not found."));
        }

        // 4. Check authorization - only the topic's supervisor can reject
        if (topic.SupervisorId != currentStaff.Id)
        {
            _logger.LogWarning("RejectApplication failed: User={UserId} is not the supervisor for Topic={TopicId}", supervisorUserId, topic.Id);
            return Result.Failure(new Error("Authorization.Forbidden", "Only the topic supervisor can reject applications."));
        }

        // 5. Check if topic is deleted (optional check, less critical than for Accept)
        if (topic.IsDeleted)
        {
            _logger.LogWarning("RejectApplication failed: Topic ID={TopicId} is deleted.", topic.Id);
            return Result.Failure(new Error("Topic.Deleted", "Cannot reject applications for a deleted topic."));
        }

        // 6. Reject the application (domain method)
        try
        {
            application.Reject(supervisorUserId, request.RejectReason);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "RejectApplication failed: Invalid state transition for Application ID={ApplicationId}", request.ApplicationId);
            return Result.Failure(new Error("Application.InvalidState", ex.Message));
        }

        // 7. Update application
        try
        {
            await _applicationRepository.UpdateAsync(application, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify student about rejection
            var body = string.IsNullOrWhiteSpace(request.RejectReason)
                ? $"Ваша заявка на тему «{topic.TitleRu}» была отклонена."
                : $"Ваша заявка на тему «{topic.TitleRu}» была отклонена. Причина: {request.RejectReason}";

            await _notificationService.SendAsync(
                userId: application.StudentId,
                title: "Заявка отклонена",
                createdBy: supervisorUserId,
                body: body,
                relatedEntityType: "TopicApplication",
                relatedEntityId: application.Id,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully rejected application ID={ApplicationId} by User={UserId}", request.ApplicationId, supervisorUserId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RejectApplication failed for Application ID={ApplicationId}", request.ApplicationId);
            return Result.Failure(new Error("Database.Error", $"Failed to reject application: {ex.Message}"));
        }
    }
}
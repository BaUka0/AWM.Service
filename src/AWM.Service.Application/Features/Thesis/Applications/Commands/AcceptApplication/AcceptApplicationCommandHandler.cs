namespace AWM.Service.Application.Features.Thesis.Applications.Commands.AcceptApplication;

using AWM.Service.Application.Features.Thesis.Works.Commands.CreateStudentWork;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for AcceptApplicationCommand.
/// Allows supervisor to accept a student's application to their topic.
/// </summary>
public sealed class AcceptApplicationCommandHandler : IRequestHandler<AcceptApplicationCommand, Result>
{
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly INotificationService _notificationService;
    private readonly ILogger<AcceptApplicationCommandHandler> _logger;

    public AcceptApplicationCommandHandler(
        ITopicApplicationRepository applicationRepository,
        ITopicRepository topicRepository,
        IMediator mediator,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider,
        INotificationService notificationService,
        ILogger<AcceptApplicationCommandHandler> logger)
    {
        _applicationRepository = applicationRepository;
        _topicRepository = topicRepository;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result> Handle(AcceptApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to accept application ID={ApplicationId} by User={UserId}", request.ApplicationId, userId);

        if (!userId.HasValue)
        {
            _logger.LogWarning("AcceptApplication failed: User identity could not be determined.");
            return Result.Failure(new Error("Authorization.Unauthorized", "User identity could not be determined."));
        }

        var supervisorUserId = userId.Value;

        // 1. Get application with topic (for authorization)
        var application = await _applicationRepository.GetByIdWithTopicAsync(
            request.ApplicationId,
            cancellationToken);

        if (application is null)
        {
            _logger.LogWarning("AcceptApplication failed: Application ID={ApplicationId} not found.", request.ApplicationId);
            return Result.Failure(new Error("Application.NotFound", $"Application with ID {request.ApplicationId} not found."));
        }

        // 2. Check if application is deleted
        if (application.IsDeleted)
        {
            _logger.LogWarning("AcceptApplication failed: Application ID={ApplicationId} is deleted.", request.ApplicationId);
            return Result.Failure(new Error("Application.Deleted", "Cannot accept a deleted application."));
        }

        // 3. Get the topic (we need it loaded separately for full checks)
        var topic = await _topicRepository.GetByIdAsync(application.TopicId, cancellationToken);
        if (topic is null)
        {
            _logger.LogWarning("AcceptApplication failed: Related topic ID={TopicId} not found for Application ID={ApplicationId}.", application.TopicId, request.ApplicationId);
            return Result.Failure(new Error("Topic.NotFound", "Related topic not found."));
        }

        // 4. Check authorization - only the topic's supervisor can accept
        if (topic.SupervisorId != supervisorUserId)
        {
            _logger.LogWarning("AcceptApplication failed: User={UserId} is not the supervisor for Topic={TopicId}", supervisorUserId, topic.Id);
            return Result.Failure(new Error("Authorization.Forbidden", "Only the topic supervisor can accept applications."));
        }

        // 5. Check if topic is still open for acceptance
        if (!topic.IsApproved)
        {
            _logger.LogWarning("AcceptApplication failed: Topic ID={TopicId} is not approved.", topic.Id);
            return Result.Failure(new Error("Topic.NotApproved", "Cannot accept applications for an unapproved topic."));
        }

        if (topic.IsClosed)
        {
            _logger.LogWarning("AcceptApplication failed: Topic ID={TopicId} is closed.", topic.Id);
            return Result.Failure(new Error("Topic.Closed", "Cannot accept applications for a closed topic."));
        }

        if (topic.IsDeleted)
        {
            _logger.LogWarning("AcceptApplication failed: Topic ID={TopicId} is deleted.", topic.Id);
            return Result.Failure(new Error("Topic.Deleted", "Cannot accept applications for a deleted topic."));
        }

        // 6. Check if there are available spots
        if (!topic.CanAcceptApplications())
        {
            _logger.LogWarning("AcceptApplication failed: Topic ID={TopicId} is full.", topic.Id);
            return Result.Failure(new Error("Topic.Full", "This topic has reached maximum participants. Cannot accept more applications."));
        }

        // 7. Accept the application (domain method)
        try
        {
            application.Accept(supervisorUserId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "AcceptApplication failed: Invalid state transition for Application ID={ApplicationId}", request.ApplicationId);
            return Result.Failure(new Error("Application.InvalidState", ex.Message));
        }

        // 8. Persist application update + create StudentWork atomically in one transaction
        _logger.LogInformation("Starting transaction to accept Application ID={ApplicationId} and create StudentWork", request.ApplicationId);
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await _applicationRepository.UpdateAsync(application, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 9. Automatically create StudentWork for the accepted student
            var createWorkCommand = new CreateStudentWorkCommand
            {
                TopicId = topic.Id,
                AcademicYearId = topic.AcademicYearId,
                DepartmentId = topic.DepartmentId,
                StudentId = application.StudentId
            };

            _logger.LogInformation("Creating student work for Topic={TopicId}, Student={StudentUserId}", topic.Id, application.StudentId);
            var workResult = await _mediator.Send(createWorkCommand, cancellationToken);

            if (workResult.IsFailed)
            {
                _logger.LogWarning("AcceptApplication failed: StudentWork creation failed. Rolling back transaction. Error: {Error}", workResult.Error.Message);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result.Failure(new Error("AcceptApplication.WorkCreationFailure",
                    $"Failed to create student work: {workResult.Error.Message}"));
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Notify student about acceptance
            await _notificationService.SendAsync(
                userId: application.StudentId,
                title: "Заявка принята",
                createdBy: supervisorUserId,
                body: $"Ваша заявка на тему «{topic.TitleRu}» была принята.",
                relatedEntityType: "TopicApplication",
                relatedEntityId: application.Id,
                cancellationToken: cancellationToken);

            // Auto-close topic if all slots are filled
            if (!topic.CanAcceptApplications())
            {
                topic.Close();
                await _topicRepository.UpdateAsync(topic, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Topic ID={TopicId} auto-closed: all slots filled.", topic.Id);
            }

            _logger.LogInformation("Successfully accepted application ID={ApplicationId} and created student work.", request.ApplicationId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AcceptApplication failed during transaction for Application ID={ApplicationId}. Rolling back.", request.ApplicationId);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure(new Error("Database.Error", $"Failed to accept application: {ex.Message}"));
        }
    }
}
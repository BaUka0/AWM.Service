namespace AWM.Service.Application.Features.Thesis.Applications.Commands.CreateApplication;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for CreateApplicationCommand.
/// Creates a new topic application for a student.
/// </summary>
public sealed class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, Result<long>>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IPeriodValidationService _periodValidationService;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateApplicationCommandHandler> _logger;

    public CreateApplicationCommandHandler(
        ITopicRepository topicRepository,
        ITopicApplicationRepository applicationRepository,
        ICurrentUserProvider currentUserProvider,
        IPeriodValidationService periodValidationService,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<CreateApplicationCommandHandler> logger)
    {
        _topicRepository = topicRepository;
        _applicationRepository = applicationRepository;
        _currentUserProvider = currentUserProvider;
        _periodValidationService = periodValidationService;
        _notificationService = notificationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<long>> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to create application for Topic ID={TopicId} by User={UserId}", request.TopicId, userId);

        if (!userId.HasValue)
        {
            _logger.LogWarning("CreateApplication failed: User identity could not be determined.");
            return Result.Failure<long>(new Error("Authorization.Unauthorized", "User identity could not be determined."));
        }

        var studentUserId = userId.Value;

        // 1. Get topic
        var topic = await _topicRepository.GetByIdAsync(request.TopicId, cancellationToken);
        if (topic is null)
        {
            _logger.LogWarning("CreateApplication failed: Topic ID={TopicId} not found.", request.TopicId);
            return Result.Failure<long>(new Error("Topic.NotFound", $"Topic with ID {request.TopicId} not found."));
        }

        // 2. Check if topic is deleted
        if (topic.IsDeleted)
        {
            _logger.LogWarning("CreateApplication failed: Topic ID={TopicId} is deleted.", request.TopicId);
            return Result.Failure<long>(new Error("Topic.Deleted", "Cannot apply to a deleted topic."));
        }

        // 3. Check if topic is approved
        if (!topic.IsApproved)
        {
            _logger.LogWarning("CreateApplication failed: Topic ID={TopicId} is not approved.", request.TopicId);
            return Result.Failure<long>(new Error("Topic.NotApproved", "This topic is not yet approved for student applications."));
        }

        // 3a. Validate that TopicSelection period is open
        var (isAllowed, errorMessage) = await _periodValidationService
            .ValidateOperationInPeriodAsync(topic.DepartmentId, topic.AcademicYearId,
                WorkflowStage.TopicSelection, cancellationToken);

        if (!isAllowed)
        {
            _logger.LogWarning("CreateApplication failed: Period closed - {Error}", errorMessage);
            return Result.Failure<long>(new Error("Period.Closed", errorMessage!));
        }

        // 4. Check if topic is closed
        if (topic.IsClosed)
        {
            _logger.LogWarning("CreateApplication failed: Topic ID={TopicId} is closed.", request.TopicId);
            return Result.Failure<long>(new Error("Topic.Closed", "This topic is closed for applications."));
        }

        // 5. Check if topic can accept more applications
        if (!topic.CanAcceptApplications())
        {
            _logger.LogWarning("CreateApplication failed: Topic ID={TopicId} maximum participants reached.", request.TopicId);
            return Result.Failure<long>(new Error("Topic.Full", "This topic has reached maximum participants."));
        }

        // 6. Check if student already applied to this topic
        var hasApplied = await _applicationRepository.HasStudentAppliedToTopicAsync(
            studentUserId,
            request.TopicId,
            cancellationToken);

        if (hasApplied)
        {
            _logger.LogWarning("CreateApplication failed: Student={UserId} already applied to Topic={TopicId}", studentUserId, request.TopicId);
            return Result.Failure<long>(new Error("Application.Duplicate", "You have already applied to this topic."));
        }

        // 7. Check if student already has an accepted application this year
        var hasAccepted = await _applicationRepository.HasAcceptedApplicationAsync(
            studentUserId,
            topic.AcademicYearId,
            cancellationToken);

        if (hasAccepted)
        {
            _logger.LogWarning("CreateApplication failed: Student={UserId} already has an accepted application for year={Year}", studentUserId, topic.AcademicYearId);
            return Result.Failure<long>(new Error("Application.AlreadyAccepted",
                "You already have an accepted application for this academic year."));
        }

        // 8. Create application
        var application = new TopicApplication(
            topicId: request.TopicId,
            studentId: studentUserId,
            motivationLetter: request.MotivationLetter
        );

        // 9. Add application to repository
        try
        {
            await _applicationRepository.AddAsync(application, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify supervisor about new application
            await _notificationService.SendAsync(
                userId: topic.SupervisorId,
                title: "Новая заявка на тему",
                createdBy: studentUserId,
                body: $"Студент подал заявку на тему «{topic.TitleRu}».",
                relatedEntityType: "TopicApplication",
                relatedEntityId: application.Id,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully created application ID={ApplicationId} for Topic ID={TopicId} by Student={UserId}",
                application.Id, request.TopicId, studentUserId);
            return Result.Success(application.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateApplication failed for Topic={TopicId} by Student={UserId}", request.TopicId, studentUserId);
            return Result.Failure<long>(new Error("Database.Error", $"Failed to create application: {ex.Message}"));
        }
    }
}
namespace AWM.Service.Application.Features.Thesis.Topics.Commands.CompleteTopicCoordination;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for completing topic coordination.
/// Closes all open topics, rejects pending applications, and notifies participants.
/// </summary>
public sealed class CompleteTopicCoordinationCommandHandler
    : IRequestHandler<CompleteTopicCoordinationCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompleteTopicCoordinationCommandHandler> _logger;

    public CompleteTopicCoordinationCommandHandler(
        ITopicRepository topicRepository,
        ITopicApplicationRepository applicationRepository,
        INotificationService notificationService,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<CompleteTopicCoordinationCommandHandler> logger)
    {
        _topicRepository = topicRepository;
        _applicationRepository = applicationRepository;
        _notificationService = notificationService;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CompleteTopicCoordinationCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Completing topic coordination for Dept={DeptId}, Year={YearId} by User={UserId}",
            request.DepartmentId, request.AcademicYearId, userId);

        if (!userId.HasValue)
        {
            return Result.Failure(new Error("401", "User ID is not available."));
        }

        var topics = await _topicRepository.GetByDepartmentAsync(
            request.DepartmentId, request.AcademicYearId, cancellationToken);

        if (topics.Count == 0)
        {
            return Result.Failure(new Error("404", "No topics found for this department and academic year."));
        }

        var supervisorIds = new HashSet<int>();
        var notifiedStudents = new HashSet<int>();

        foreach (var topic in topics)
        {
            if (topic.IsDeleted) continue;

            supervisorIds.Add(topic.SupervisorId);

            // Close all open topics
            if (!topic.IsClosed)
            {
                topic.Close();
                await _topicRepository.UpdateAsync(topic, cancellationToken);
            }

            // Reject all pending applications
            var applications = await _applicationRepository.GetByTopicIdAsync(topic.Id, cancellationToken);
            foreach (var app in applications)
            {
                if (app.Status == ApplicationStatus.Submitted)
                {
                    app.Reject(userId.Value, "Этап согласования тем завершён.");
                    await _applicationRepository.UpdateAsync(app, cancellationToken);
                    notifiedStudents.Add(app.StudentId);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify supervisors
        if (supervisorIds.Count > 0)
        {
            await _notificationService.SendToManyAsync(
                userIds: supervisorIds,
                title: "Согласование тем завершено",
                createdBy: userId.Value,
                body: "Этап согласования тем завершён. Все темы закрыты для приёма заявок.",
                relatedEntityType: "Department",
                relatedEntityId: request.DepartmentId,
                cancellationToken: cancellationToken);
        }

        // Notify students whose pending applications were rejected
        if (notifiedStudents.Count > 0)
        {
            await _notificationService.SendToManyAsync(
                userIds: notifiedStudents,
                title: "Заявка отклонена",
                createdBy: userId.Value,
                body: "Ваша заявка была отклонена в связи с завершением этапа согласования тем.",
                cancellationToken: cancellationToken);
        }

        _logger.LogInformation(
            "Topic coordination completed for Dept={DeptId}. Closed {TopicCount} topics, rejected {StudentCount} pending apps.",
            request.DepartmentId, topics.Count, notifiedStudents.Count);

        return Result.Success();
    }
}

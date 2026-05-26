using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.CommonDomain.Events;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Events;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.University;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Notifications.EventHandlers;

/// <summary>
/// Domain event handlers for generating automated workflow notifications.
/// Bridges the gap between workflow states and user notifications.
/// </summary>
public sealed class WorkflowNotificationHandlers :
    INotificationHandler<ApplicationSubmittedEvent>,
    INotificationHandler<ApplicationAcceptedEvent>,
    INotificationHandler<ApplicationRejectedEvent>,
    INotificationHandler<DirectionCreatedEvent>,
    INotificationHandler<DirectionSubmittedEvent>,
    INotificationHandler<DirectionApprovedEvent>,
    INotificationHandler<DirectionRejectedEvent>,
    INotificationHandler<DirectionRequiresRevisionEvent>,
    INotificationHandler<TopicCreatedEvent>,
    INotificationHandler<TopicApprovedEvent>,
    INotificationHandler<TopicsSubmittedForApprovalEvent>,
    INotificationHandler<TopicReconciliationCompletedEvent>,
    INotificationHandler<WorkCreatedEvent>,
    INotificationHandler<WorkStateChangedEvent>,
    INotificationHandler<QualityCheckCompletedEvent>,
    INotificationHandler<WorkDefendedEvent>,
    INotificationHandler<SupervisorsApprovedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ITopicRepository _topicRepository;
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IDirectionRepository _directionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public WorkflowNotificationHandlers(
        INotificationService notificationService,
        ITopicRepository topicRepository,
        IStudentWorkRepository studentWorkRepository,
        IDirectionRepository directionRepository,
        IUserRepository userRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        IWorkflowRepository workflowRepository)
    {
        _notificationService = notificationService;
        _topicRepository = topicRepository;
        _studentWorkRepository = studentWorkRepository;
        _directionRepository = directionRepository;
        _userRepository = userRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
        _workflowRepository = workflowRepository;
    }

    /// <inheritdoc />
    public async Task Handle(ApplicationSubmittedEvent notification, CancellationToken cancellationToken)
    {
        var topic = await _topicRepository.GetByIdAsync(notification.TopicId, cancellationToken);
        if (topic == null) return;

        var student = await _userRepository.GetByIdAsync(notification.StudentId, cancellationToken);
        var studentName = student != null ? $"{student.LastName} {student.FirstName}".Trim() : $"Студент #{notification.StudentId}";

        await _notificationService.SendAsync(
            userId: topic.CreatedBy,
            title: "Новая заявка на тему",
            createdBy: notification.StudentId,
            body: $"Студент {studentName} подал заявку на вашу тему '{topic.TitleRu}'.",
            relatedEntityType: "TopicApplication",
            relatedEntityId: notification.ApplicationId,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task Handle(ApplicationAcceptedEvent notification, CancellationToken cancellationToken)
    {
        var topic = await _topicRepository.GetByIdAsync(notification.TopicId, cancellationToken);
        if (topic == null) return;

        await _notificationService.SendAsync(
            userId: notification.StudentId,
            title: "Заявка на тему принята",
            createdBy: notification.ReviewedBy,
            body: $"Ваша заявка на тему '{topic.TitleRu}' была принята научным руководителем.",
            relatedEntityType: "TopicApplication",
            relatedEntityId: notification.ApplicationId,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task Handle(ApplicationRejectedEvent notification, CancellationToken cancellationToken)
    {
        var topic = await _topicRepository.GetByIdAsync(notification.TopicId, cancellationToken);
        if (topic == null) return;

        var reasonText = !string.IsNullOrWhiteSpace(notification.Reason) ? $". Причина: {notification.Reason}" : ".";

        await _notificationService.SendAsync(
            userId: notification.StudentId,
            title: "Заявка на тему отклонена",
            createdBy: notification.ReviewedBy,
            body: $"Ваша заявка на тему '{topic.TitleRu}' была отклонена научным руководителем{reasonText}",
            relatedEntityType: "TopicApplication",
            relatedEntityId: notification.ApplicationId,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task Handle(DirectionCreatedEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task Handle(DirectionSubmittedEvent notification, CancellationToken cancellationToken)
    {
        var direction = await _directionRepository.GetByIdAsync(notification.DirectionId, cancellationToken);
        if (direction == null) return;

        // Fetch department chairmen & secretaries
        var chairmen = await _staffAssignmentRepository.GetByRoleAsync("OrgUnit", direction.OrgUnitId, StaffRoleType.CommissionChairman, cancellationToken);
        var secretaries = await _staffAssignmentRepository.GetByRoleAsync("OrgUnit", direction.OrgUnitId, StaffRoleType.CommissionSecretary, cancellationToken);
        var departmentUsersToNotify = chairmen.Concat(secretaries).Select(a => a.UserId).Distinct().ToList();

        if (departmentUsersToNotify.Any())
        {
            await _notificationService.SendToManyAsync(
                userIds: departmentUsersToNotify,
                title: "Направление представлено на утверждение",
                createdBy: direction.CreatedBy,
                body: $"Руководитель представил новое направление '{direction.TitleRu}' на утверждение кафедры.",
                relatedEntityType: "Direction",
                relatedEntityId: notification.DirectionId,
                cancellationToken: cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task Handle(DirectionApprovedEvent notification, CancellationToken cancellationToken)
    {
        var direction = await _directionRepository.GetByIdAsync(notification.DirectionId, cancellationToken);
        if (direction == null) return;

        await _notificationService.SendAsync(
            userId: direction.CreatedBy,
            title: "Направление утверждено",
            createdBy: notification.ReviewedBy,
            body: $"Ваше направление '{direction.TitleRu}' было успешно утверждено кафедрой.",
            relatedEntityType: "Direction",
            relatedEntityId: notification.DirectionId,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task Handle(DirectionRejectedEvent notification, CancellationToken cancellationToken)
    {
        var direction = await _directionRepository.GetByIdAsync(notification.DirectionId, cancellationToken);
        if (direction == null) return;

        var commentText = !string.IsNullOrWhiteSpace(notification.Comment) ? $". Комментарий: {notification.Comment}" : ".";

        await _notificationService.SendAsync(
            userId: direction.CreatedBy,
            title: "Направление отклонено",
            createdBy: notification.ReviewedBy,
            body: $"Ваше направление '{direction.TitleRu}' было отклонено{commentText}",
            relatedEntityType: "Direction",
            relatedEntityId: notification.DirectionId,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task Handle(DirectionRequiresRevisionEvent notification, CancellationToken cancellationToken)
    {
        var direction = await _directionRepository.GetByIdAsync(notification.DirectionId, cancellationToken);
        if (direction == null) return;

        await _notificationService.SendAsync(
            userId: direction.CreatedBy,
            title: "Направление требует доработки",
            createdBy: notification.ReviewedBy,
            body: $"Ваше направление '{direction.TitleRu}' отправлено на доработку. Комментарий: {notification.Comment}",
            relatedEntityType: "Direction",
            relatedEntityId: notification.DirectionId,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task Handle(TopicCreatedEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task Handle(TopicApprovedEvent notification, CancellationToken cancellationToken)
    {
        var topic = await _topicRepository.GetByIdAsync(notification.TopicId, cancellationToken);
        if (topic == null) return;

        await _notificationService.SendAsync(
            userId: topic.CreatedBy,
            title: "Тема утверждена",
            createdBy: topic.ReviewedBy ?? 1,
            body: $"Ваша тема '{topic.TitleRu}' была утверждена кафедрой.",
            relatedEntityType: "Topic",
            relatedEntityId: notification.TopicId,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task Handle(TopicsSubmittedForApprovalEvent notification, CancellationToken cancellationToken)
    {
        if (!notification.TopicIds.Any()) return;
        var firstTopic = await _topicRepository.GetByIdAsync(notification.TopicIds.First(), cancellationToken);
        if (firstTopic == null) return;

        var chairmen = await _staffAssignmentRepository.GetByRoleAsync("OrgUnit", firstTopic.OrgUnitId, StaffRoleType.CommissionChairman, cancellationToken);
        var secretaries = await _staffAssignmentRepository.GetByRoleAsync("OrgUnit", firstTopic.OrgUnitId, StaffRoleType.CommissionSecretary, cancellationToken);
        var departmentUsersToNotify = chairmen.Concat(secretaries).Select(a => a.UserId).Distinct().ToList();

        if (departmentUsersToNotify.Any())
        {
            await _notificationService.SendToManyAsync(
                userIds: departmentUsersToNotify,
                title: "Темы представлены на утверждение",
                createdBy: notification.SupervisorId,
                body: $"Руководитель представил {notification.TopicIds.Count} тем(ы) на утверждение кафедры.",
                relatedEntityType: "Topic",
                relatedEntityId: notification.TopicIds.First(),
                cancellationToken: cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task Handle(TopicReconciliationCompletedEvent notification, CancellationToken cancellationToken)
    {
        var studentWorks = await _studentWorkRepository.GetByOrgUnitAsync(notification.OrgUnitId, notification.SemesterId, cancellationToken);
        var studentIds = studentWorks.SelectMany(w => w.Participants.Select(p => p.StudentId)).Distinct().ToList();

        if (studentIds.Any())
        {
            await _notificationService.SendToManyAsync(
                userIds: studentIds,
                title: "Согласование тем завершено",
                createdBy: notification.CompletedBy,
                body: "Согласование тем в вашей группе/кафедре успешно завершено. Для вас создана дипломная работа.",
                cancellationToken: cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task Handle(WorkCreatedEvent notification, CancellationToken cancellationToken)
    {
        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(notification.WorkId, cancellationToken);
        if (work == null) return;

        Topic? topic = null;
        if (work.TopicId.HasValue)
        {
            topic = await _topicRepository.GetByIdAsync(work.TopicId.Value, cancellationToken);
        }
        var topicTitle = topic != null ? topic.TitleRu : "Выпускная квалификационная работа";

        var studentIds = work.Participants.Select(p => p.StudentId).ToList();
        if (studentIds.Any())
        {
            await _notificationService.SendToManyAsync(
                userIds: studentIds,
                title: "Создана выпускная работа",
                createdBy: work.CreatedBy,
                body: $"Для вас создана выпускная работа по теме '{topicTitle}'.",
                relatedEntityType: "StudentWork",
                relatedEntityId: notification.WorkId,
                cancellationToken: cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task Handle(WorkStateChangedEvent notification, CancellationToken cancellationToken)
    {
        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(notification.WorkId, cancellationToken);
        if (work == null) return;

        var targetState = await _workflowRepository.GetStateByIdAsync(notification.ToStateId, cancellationToken);
        if (targetState == null) return;

        var studentIds = work.Participants.Select(p => p.StudentId).ToList();
        if (studentIds.Any())
        {
            await _notificationService.SendToManyAsync(
                userIds: studentIds,
                title: "Изменение статуса работы",
                createdBy: notification.ChangedBy,
                body: $"Статус вашей работы изменен на '{targetState.DisplayName}'.",
                relatedEntityType: "StudentWork",
                relatedEntityId: notification.WorkId,
                cancellationToken: cancellationToken);
        }

        if (work.TopicId.HasValue)
        {
            var topic = await _topicRepository.GetByIdAsync(work.TopicId.Value, cancellationToken);
            if (topic != null)
            {
                await _notificationService.SendAsync(
                    userId: topic.CreatedBy,
                    title: "Изменение статуса работы студента",
                    createdBy: notification.ChangedBy,
                    body: $"Статус работы по теме '{topic.TitleRu}' изменен на '{targetState.DisplayName}'.",
                    relatedEntityType: "StudentWork",
                    relatedEntityId: notification.WorkId,
                    cancellationToken: cancellationToken);
            }
        }
    }

    /// <inheritdoc />
    public async Task Handle(QualityCheckCompletedEvent notification, CancellationToken cancellationToken)
    {
        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(notification.WorkId, cancellationToken);
        if (work == null) return;

        var studentIds = work.Participants.Select(p => p.StudentId).ToList();
        if (studentIds.Any())
        {
            var outcome = notification.IsPassed ? "успешно пройдена" : "не пройдена (требуется доработка)";
            await _notificationService.SendToManyAsync(
                userIds: studentIds,
                title: "Проверка качества завершена",
                createdBy: notification.ExpertId,
                body: $"Проверка типа '{notification.CheckType}' была {outcome}.",
                relatedEntityType: "StudentWork",
                relatedEntityId: notification.WorkId,
                cancellationToken: cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task Handle(WorkDefendedEvent notification, CancellationToken cancellationToken)
    {
        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(notification.WorkId, cancellationToken);
        if (work == null) return;

        var studentIds = work.Participants.Select(p => p.StudentId).ToList();
        if (studentIds.Any())
        {
            await _notificationService.SendToManyAsync(
                userIds: studentIds,
                title: "Защита дипломной работы завершена",
                createdBy: work.CreatedBy,
                body: $"Поздравляем с завершением защиты! Ваша итоговая оценка: {notification.FinalGrade ?? "Отлично"}.",
                relatedEntityType: "StudentWork",
                relatedEntityId: notification.WorkId,
                cancellationToken: cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task Handle(SupervisorsApprovedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.SupervisorUserIds.Any())
        {
            await _notificationService.SendToManyAsync(
                userIds: notification.SupervisorUserIds.ToList(),
                title: "Назначение научным руководителем",
                createdBy: notification.ConfirmedBy,
                body: "Вы были утверждены в качестве научного руководителя на текущий период.",
                relatedEntityType: "OrgUnit",
                relatedEntityId: notification.OrgUnitId,
                cancellationToken: cancellationToken);
        }
    }
}

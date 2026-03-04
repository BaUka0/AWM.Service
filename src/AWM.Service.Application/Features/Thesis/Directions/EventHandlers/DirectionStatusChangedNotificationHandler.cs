namespace AWM.Service.Application.Features.Thesis.Directions.EventHandlers;

using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Events;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Sends notification to the supervisor when a direction is approved.
/// </summary>
public sealed class DirectionApprovedNotificationHandler : INotificationHandler<DirectionApprovedEvent>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<DirectionApprovedNotificationHandler> _logger;

    public DirectionApprovedNotificationHandler(
        IDirectionRepository directionRepository,
        IStaffRepository staffRepository,
        INotificationService notificationService,
        ILogger<DirectionApprovedNotificationHandler> logger)
    {
        _directionRepository = directionRepository;
        _staffRepository = staffRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(DirectionApprovedEvent notification, CancellationToken cancellationToken)
    {
        var direction = await _directionRepository.GetByIdAsync(notification.DirectionId, cancellationToken);
        if (direction is null) return;

        var staff = await _staffRepository.GetByIdAsync(direction.SupervisorId, cancellationToken);
        if (staff is null)
        {
            _logger.LogWarning("Cannot send notification: Staff {StaffId} not found for direction {DirectionId}", direction.SupervisorId, direction.Id);
            return;
        }

        await _notificationService.SendAsync(
            userId: staff.UserId,
            title: "Направление утверждено",
            createdBy: notification.ReviewedBy,
            body: $"Ваше направление «{direction.TitleRu}» было утверждено.",
            relatedEntityType: "Direction",
            relatedEntityId: direction.Id,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Notification sent to supervisor UserId={UserId} (StaffId={StaffId}) about direction {DirectionId} approval",
            staff.UserId, direction.SupervisorId, direction.Id);
    }
}

/// <summary>
/// Sends notification to the supervisor when a direction is rejected.
/// </summary>
public sealed class DirectionRejectedNotificationHandler : INotificationHandler<DirectionRejectedEvent>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<DirectionRejectedNotificationHandler> _logger;

    public DirectionRejectedNotificationHandler(
        IDirectionRepository directionRepository,
        IStaffRepository staffRepository,
        INotificationService notificationService,
        ILogger<DirectionRejectedNotificationHandler> logger)
    {
        _directionRepository = directionRepository;
        _staffRepository = staffRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(DirectionRejectedEvent notification, CancellationToken cancellationToken)
    {
        var direction = await _directionRepository.GetByIdAsync(notification.DirectionId, cancellationToken);
        if (direction is null) return;

        var staff = await _staffRepository.GetByIdAsync(direction.SupervisorId, cancellationToken);
        if (staff is null)
        {
            _logger.LogWarning("Cannot send notification: Staff {StaffId} not found for direction {DirectionId}", direction.SupervisorId, direction.Id);
            return;
        }

        var body = string.IsNullOrWhiteSpace(notification.Comment)
            ? $"Ваше направление «{direction.TitleRu}» было отклонено."
            : $"Ваше направление «{direction.TitleRu}» было отклонено. Причина: {notification.Comment}";

        await _notificationService.SendAsync(
            userId: staff.UserId,
            title: "Направление отклонено",
            createdBy: notification.ReviewedBy,
            body: body,
            relatedEntityType: "Direction",
            relatedEntityId: direction.Id,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Notification sent to supervisor UserId={UserId} (StaffId={StaffId}) about direction {DirectionId} rejection",
            staff.UserId, direction.SupervisorId, direction.Id);
    }
}

/// <summary>
/// Sends notification to the supervisor when a direction requires revision.
/// </summary>
public sealed class DirectionRequiresRevisionNotificationHandler : INotificationHandler<DirectionRequiresRevisionEvent>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<DirectionRequiresRevisionNotificationHandler> _logger;

    public DirectionRequiresRevisionNotificationHandler(
        IDirectionRepository directionRepository,
        IStaffRepository staffRepository,
        INotificationService notificationService,
        ILogger<DirectionRequiresRevisionNotificationHandler> logger)
    {
        _directionRepository = directionRepository;
        _staffRepository = staffRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(DirectionRequiresRevisionEvent notification, CancellationToken cancellationToken)
    {
        var direction = await _directionRepository.GetByIdAsync(notification.DirectionId, cancellationToken);
        if (direction is null) return;

        var staff = await _staffRepository.GetByIdAsync(direction.SupervisorId, cancellationToken);
        if (staff is null)
        {
            _logger.LogWarning("Cannot send notification: Staff {StaffId} not found for direction {DirectionId}", direction.SupervisorId, direction.Id);
            return;
        }

        await _notificationService.SendAsync(
            userId: staff.UserId,
            title: "Направление требует доработки",
            createdBy: notification.ReviewedBy,
            body: $"Ваше направление «{direction.TitleRu}» требует доработки. Комментарий: {notification.Comment}",
            relatedEntityType: "Direction",
            relatedEntityId: direction.Id,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Notification sent to supervisor UserId={UserId} (StaffId={StaffId}) about direction {DirectionId} revision request",
            staff.UserId, direction.SupervisorId, direction.Id);
    }
}

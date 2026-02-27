namespace AWM.Service.WebAPI.Controllers.v1;

using AWM.Service.Application.Features.Common.Notifications.Commands.MarkAllAsRead;
using AWM.Service.Application.Features.Common.Notifications.Commands.MarkAsRead;
using AWM.Service.Application.Features.Common.Notifications.Queries.GetMyNotifications;
using AWM.Service.Domain.Auth.Enums;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.WebAPI.Authorization;
using AWM.Service.WebAPI.Common.Contracts.Responses.Common;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller for managing the current user's notifications (bell icon).
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces("application/json")]
public sealed class NotificationsController : BaseController
{
    private readonly ISender _sender;
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public NotificationsController(
        ISender sender,
        INotificationRepository notificationRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _sender = sender;
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    /// <summary>
    /// Get notifications for the currently authenticated user.
    /// </summary>
    /// <param name="skip">Number of notifications to skip (for pagination).</param>
    /// <param name="take">Number of notifications to return (default: 20).</param>
    /// <param name="onlyUnread">If true, returns only unread notifications.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of notifications with unread count summary.</returns>
    [HttpGet]
    [RequirePermission(Permission.Notifications_View)]
    [ProducesResponseType(typeof(NotificationListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyNotifications(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        [FromQuery] bool? onlyUnread = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetMyNotificationsQuery
        {
            Skip = skip,
            Take = take,
            OnlyUnread = onlyUnread
        };

        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        var items = result.Value.Adapt<List<NotificationResponse>>();

        var unreadCount = items.Count(n => !n.IsRead);

        var response = new NotificationListResponse
        {
            UnreadCount = unreadCount,
            Items = items
        };

        return Ok(response);
    }

    /// <summary>
    /// Mark a specific notification as read.
    /// </summary>
    /// <param name="notificationId">Notification ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPatch("{notificationId:long}/read")]
    [RequirePermission(Permission.Notifications_MarkRead)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkAsRead(long notificationId, CancellationToken cancellationToken = default)
    {
        var command = new MarkAsReadCommand
        {
            NotificationId = notificationId
        };

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }

    /// <summary>
    /// Mark all notifications of the current user as read.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpPatch("read-all")]
    [RequirePermission(Permission.Notifications_MarkRead)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default)
    {
        var command = new MarkAllAsReadCommand();

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
            return HandleResultError(result.Error);

        return NoContent();
    }

    /// <summary>
    /// Get the count of unread notifications for the currently authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Unread notification count.</returns>
    [HttpGet("unread-count")]
    [RequirePermission(Permission.Notifications_View)]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken = default)
    {
        var userId = _currentUserProvider.UserId;
        if (!userId.HasValue)
            return Unauthorized();

        var count = await _notificationRepository.GetUnreadCountAsync(userId.Value, cancellationToken);
        return Ok(count);
    }
}

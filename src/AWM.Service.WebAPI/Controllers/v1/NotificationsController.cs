using AWM.Service.Application.Features.Notifications.Commands.MarkAllAsRead;
using AWM.Service.Application.Features.Notifications.Commands.MarkAsRead;
using AWM.Service.Application.Features.Notifications.Queries.GetNotifications;
using AWM.Service.Application.Features.Notifications.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.WebAPI.Controllers.v1;

/// <summary>
/// Controller for managing personal user notifications (колокольчик).
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications")]
[ApiController]
[Authorize]
public sealed class NotificationsController : BaseController
{
    private readonly ISender _sender;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationsController"/> class.
    /// </summary>
    /// <param name="sender">The MediatR sender.</param>
    public NotificationsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets personal notifications for the authenticated user.
    /// </summary>
    /// <param name="skip">Number of records to skip.</param>
    /// <param name="take">Number of records to take.</param>
    /// <param name="unreadOnly">If true, returns only unread notifications.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of notification DTOs.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        [FromQuery] bool unreadOnly = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetNotificationsQuery(skip, take, unreadOnly);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Marks a specific notification as read.
    /// </summary>
    /// <param name="id">Notification identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP 200 OK if successful.</returns>
    [HttpPost("{id:long}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(
        long id,
        CancellationToken cancellationToken = default)
    {
        var command = new MarkNotificationAsReadCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Marks all notifications for the current user as read.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>HTTP 200 OK if successful.</returns>
    [HttpPost("read-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken = default)
    {
        var command = new MarkAllNotificationsAsReadCommand();
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok();
    }

    /// <summary>
    /// Gets the count of unread personal notifications for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A number of unread notifications.</returns>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken = default)
    {
        var query = new AWM.Service.Application.Features.Notifications.Queries.GetUnreadNotificationsCount.GetUnreadNotificationsCountQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailed)
        {
            return HandleResultError(result.Error);
        }

        return Ok(result.Value);
    }
}

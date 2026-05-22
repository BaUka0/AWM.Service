namespace AWM.Service.Application.Features.Common.Notifications.Queries.GetUnreadCount;

using MediatR;

/// <summary>
/// Query to get the count of unread notifications for the current user.
/// </summary>
public sealed record GetUnreadNotificationsCountQuery(int UserId) : IRequest<int>;

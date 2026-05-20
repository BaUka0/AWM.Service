namespace AWM.Service.Application.Features.Common.Notifications.Queries.GetMyNotifications;

using AWM.Service.Application.Features.Common.Notifications.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetMyNotificationsQuery : IRequest<Result<(IReadOnlyList<NotificationDto> Items, int TotalCount, int TotalUnreadCount)>>
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public bool? OnlyUnread { get; init; }
}

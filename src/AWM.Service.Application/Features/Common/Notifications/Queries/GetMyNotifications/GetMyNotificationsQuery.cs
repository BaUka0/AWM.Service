namespace AWM.Service.Application.Features.Common.Notifications.Queries.GetMyNotifications;

using AWM.Service.Application.Features.Common.Notifications.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed record GetMyNotificationsQuery : IRequest<Result<IReadOnlyList<NotificationDto>>>
{
    public int Skip { get; init; } = 0;
    public int Take { get; init; } = 20;
    public bool? OnlyUnread { get; init; }
}

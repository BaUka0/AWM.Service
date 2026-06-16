using AWM.Service.Application.Features.Notifications.DTOs;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;

namespace AWM.Service.Application.Features.Notifications.Queries.GetNotifications;

public sealed record GetNotificationsQuery(int Skip = 0, int Take = 20, bool UnreadOnly = false) : IRequest<Result<IReadOnlyList<NotificationDto>>>;

using AWM.Service.Application.Features.Notifications.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Notifications.Queries.GetNotifications;

public sealed class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, Result<IReadOnlyList<NotificationDto>>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetNotificationsQueryHandler(
        INotificationRepository notificationRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _notificationRepository = notificationRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<IReadOnlyList<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.IsAuthenticated || !_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<NotificationDto>>(new Error("Auth.MissingPrincipal", "Unable to identify the current user."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var notifications = request.UnreadOnly
            ? await _notificationRepository.GetUnreadByUserAsync(currentUserId, cancellationToken)
            : await _notificationRepository.GetByUserAsync(currentUserId, request.Skip, request.Take, cancellationToken);

        var dtos = notifications
            .Select(n => new NotificationDto(
                n.Id,
                n.UserId,
                n.Title,
                n.Body,
                n.IsRead,
                n.CreatedAt,
                n.RelatedEntityType,
                n.RelatedEntityId
            ))
            .ToList();

        return Result.Success<IReadOnlyList<NotificationDto>>(dtos);
    }
}

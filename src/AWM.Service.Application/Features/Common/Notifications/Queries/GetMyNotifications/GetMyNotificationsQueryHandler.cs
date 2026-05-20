namespace AWM.Service.Application.Features.Common.Notifications.Queries.GetMyNotifications;

using AWM.Service.Application.Features.Common.Notifications.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetMyNotificationsQueryHandler
    : IRequestHandler<GetMyNotificationsQuery, Result<(IReadOnlyList<NotificationDto> Items, int TotalCount, int TotalUnreadCount)>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetMyNotificationsQueryHandler(
        INotificationRepository notificationRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<(IReadOnlyList<NotificationDto> Items, int TotalCount, int TotalUnreadCount)>> Handle(
        GetMyNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<(IReadOnlyList<NotificationDto> Items, int TotalCount, int TotalUnreadCount)>(
                    new Error("401", "User is not authenticated."));

            int totalCount;
            IReadOnlyList<Domain.CommonDomain.Entities.Notification> notifications;

            int skip = (request.Page - 1) * request.PageSize;
            int take = request.PageSize;

            if (request.OnlyUnread == true)
            {
                var allUnread = await _notificationRepository.GetUnreadByUserAsync(userId.Value, cancellationToken);
                totalCount = allUnread.Count;
                notifications = allUnread.Skip(skip).Take(take).ToList();
            }
            else
            {
                totalCount = await _notificationRepository.GetCountByUserAsync(userId.Value, cancellationToken);
                notifications = await _notificationRepository.GetByUserAsync(
                    userId.Value,
                    skip,
                    take,
                    cancellationToken);
            }

            int totalUnreadCount = await _notificationRepository.GetUnreadCountAsync(userId.Value, cancellationToken);

            var dtos = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                UserId = n.UserId,
                TemplateId = n.TemplateId,
                Title = n.Title,
                Body = n.Body,
                RelatedEntityType = n.RelatedEntityType,
                RelatedEntityId = n.RelatedEntityId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                CreatedBy = n.CreatedBy,
                LastModifiedAt = n.LastModifiedAt,
                LastModifiedBy = n.LastModifiedBy
            }).ToList();

            return Result.Success<(IReadOnlyList<NotificationDto> Items, int TotalCount, int TotalUnreadCount)>((dtos, totalCount, totalUnreadCount));
        }
        catch (Exception ex)
        {
            return Result.Failure<(IReadOnlyList<NotificationDto> Items, int TotalCount, int TotalUnreadCount)>(
                new Error("500", $"An error occurred while retrieving notifications: {ex.Message}"));
        }
    }
}

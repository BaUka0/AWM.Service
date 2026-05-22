namespace AWM.Service.Application.Features.Common.Notifications.Queries.GetUnreadCount;

using AWM.Service.Domain.Repositories;
using MediatR;

/// <summary>
/// Handler for GetUnreadNotificationsCountQuery.
/// </summary>
public sealed class GetUnreadNotificationsCountQueryHandler : IRequestHandler<GetUnreadNotificationsCountQuery, int>
{
    private readonly INotificationRepository _notificationRepository;

    public GetUnreadNotificationsCountQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
    }

    public async Task<int> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
    {
        return await _notificationRepository.GetUnreadCountAsync(request.UserId, cancellationToken);
    }
}

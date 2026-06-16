using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Notifications.Queries.GetUnreadNotificationsCount;

public sealed class GetUnreadNotificationsCountQueryHandler : IRequestHandler<GetUnreadNotificationsCountQuery, Result<int>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetUnreadNotificationsCountQueryHandler(
        INotificationRepository notificationRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _notificationRepository = notificationRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<int>> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.IsAuthenticated || !_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<int>(new Error("Auth.MissingPrincipal", "Unable to identify the current user."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var unreadNotifications = await _notificationRepository.GetUnreadByUserAsync(currentUserId, cancellationToken);

        return Result.Success(unreadNotifications.Count);
    }
}

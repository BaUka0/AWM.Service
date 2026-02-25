namespace AWM.Service.Application.Features.Common.Notifications.Commands.MarkAsRead;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, Result>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public MarkAsReadCommandHandler(
        INotificationRepository notificationRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User is not authenticated."));

            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);

            if (notification is null)
                return Result.Failure(new Error("404", $"Notification with ID {request.NotificationId} not found."));

            if (notification.UserId != userId.Value)
                return Result.Failure(new Error("403", "You do not have permission to modify this notification."));

            if (notification.IsRead)
                return Result.Success();

            notification.MarkAsRead();
            await _notificationRepository.UpdateAsync(notification, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", $"An error occurred while marking the notification as read: {ex.Message}"));
        }
    }
}

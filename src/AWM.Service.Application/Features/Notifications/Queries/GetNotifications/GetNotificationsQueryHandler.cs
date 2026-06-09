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
    private readonly IUserReadOnlyRepository _userRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IDirectionRepository _directionRepository;

    public GetNotificationsQueryHandler(
        INotificationRepository notificationRepository,
        ICurrentUserProvider currentUserProvider,
        IUserReadOnlyRepository userRepository,
        ITopicRepository topicRepository,
        IDirectionRepository directionRepository)
    {
        _notificationRepository = notificationRepository;
        _currentUserProvider = currentUserProvider;
        _userRepository = userRepository;
        _topicRepository = topicRepository;
        _directionRepository = directionRepository;
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

        var actorIds = notifications.Where(n => n.CreatedBy > 0).Select(n => n.CreatedBy).Distinct().ToList();
        var actors = actorIds.Any()
            ? await _userRepository.GetByIdsAsync(actorIds, cancellationToken)
            : new List<AWM.Service.Domain.University.User>();
        var actorMap = actors.ToDictionary(a => a.Id, a => $"{a.LastName} {a.FirstName} {a.MiddleName}".Trim());

        var topicIds = notifications.Where(n => n.RelatedEntityType == "Topic" && n.RelatedEntityId.HasValue)
                                   .Select(n => n.RelatedEntityId!.Value).Distinct().ToList();
        var directionIds = notifications.Where(n => n.RelatedEntityType == "Direction" && n.RelatedEntityId.HasValue)
                                      .Select(n => n.RelatedEntityId!.Value).Distinct().ToList();

        var topics = topicIds.Any() ? await _topicRepository.GetByIdsAsync(topicIds, cancellationToken) : new List<AWM.Service.Domain.Thesis.Entities.Topic>();
        var directions = directionIds.Any() ? await _directionRepository.GetByIdsAsync(directionIds, cancellationToken) : new List<AWM.Service.Domain.Thesis.Entities.Direction>();

        var titleMap = topics.ToDictionary(t => ("Topic", t.Id), t => t.TitleRu);
        foreach (var d in directions) titleMap[("Direction", d.Id)] = d.TitleRu;

        var dtos = notifications
            .Select(n => new NotificationDto(
                n.Id,
                n.UserId,
                n.Title,
                n.Body,
                n.IsRead,
                n.CreatedAt,
                n.RelatedEntityType,
                n.RelatedEntityId,
                actorMap.GetValueOrDefault(n.CreatedBy, "System"),
                n.RelatedEntityId.HasValue ? titleMap.GetValueOrDefault((n.RelatedEntityType, n.RelatedEntityId.Value)) : null
            ))
            .ToList();

        return Result.Success<IReadOnlyList<NotificationDto>>(dtos);
    }
}

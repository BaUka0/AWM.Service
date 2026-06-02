using System;

namespace AWM.Service.WebAPI.Common.Contracts.Responses.Notifications;

public record NotificationResponse(
    long Id,
    int UserId,
    string Title,
    string Body,
    bool IsRead,
    DateTime CreatedAt,
    string RelatedEntityType,
    string RelatedEntityId
);

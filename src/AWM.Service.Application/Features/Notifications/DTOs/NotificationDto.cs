using System;

namespace AWM.Service.Application.Features.Notifications.DTOs;

public sealed record NotificationDto(
    long Id,
    int UserId,
    string Title,
    string? Body,
    bool IsRead,
    DateTime CreatedAt,
    string? RelatedEntityType,
    long? RelatedEntityId
);

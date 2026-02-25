namespace AWM.Service.Application.Features.Common.Notifications.Commands.MarkAsRead;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record MarkAsReadCommand : IRequest<Result>
{
    public long NotificationId { get; init; }
}

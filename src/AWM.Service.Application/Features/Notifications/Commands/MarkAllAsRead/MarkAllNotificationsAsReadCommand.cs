using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Notifications.Commands.MarkAllAsRead;

public sealed record MarkAllNotificationsAsReadCommand() : IRequest<Result>;

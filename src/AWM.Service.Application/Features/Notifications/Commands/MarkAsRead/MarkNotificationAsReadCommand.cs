using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Notifications.Commands.MarkAsRead;

public sealed record MarkNotificationAsReadCommand(long Id) : IRequest<Result>;

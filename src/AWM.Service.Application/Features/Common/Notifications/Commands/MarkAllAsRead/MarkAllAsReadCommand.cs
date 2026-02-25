namespace AWM.Service.Application.Features.Common.Notifications.Commands.MarkAllAsRead;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record MarkAllAsReadCommand : IRequest<Result>;

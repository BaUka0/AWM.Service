using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Notifications.Queries.GetUnreadNotificationsCount;

public sealed record GetUnreadNotificationsCountQuery() : IRequest<Result<int>>;

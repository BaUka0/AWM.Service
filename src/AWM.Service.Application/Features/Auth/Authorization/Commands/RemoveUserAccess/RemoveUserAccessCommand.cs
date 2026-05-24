namespace AWM.Service.Application.Features.Auth.Auth.Commands.RemoveUserAccess;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record RemoveUserAccessCommand : IRequest<Result>
{
    public int UserAccessId { get; init; }
}

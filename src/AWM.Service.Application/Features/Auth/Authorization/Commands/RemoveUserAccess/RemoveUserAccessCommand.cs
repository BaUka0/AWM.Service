namespace AWM.Service.Application.Features.Auth.Auth.Commands.RemoveUserAccess;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to remove a role access from a user.
/// </summary>
public sealed record RemoveUserAccessCommand : IRequest<Result>
{
    public int UserAccessId { get; init; }
}

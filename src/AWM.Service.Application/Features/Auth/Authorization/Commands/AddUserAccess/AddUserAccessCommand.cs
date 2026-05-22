namespace AWM.Service.Application.Features.Auth.Auth.Commands.AddUserAccess;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to assign a role access to a user.
/// </summary>
public sealed record AddUserAccessCommand : IRequest<Result<int>>
{
    public int UserId { get; init; }
    public int RoleAccessId { get; init; }
}

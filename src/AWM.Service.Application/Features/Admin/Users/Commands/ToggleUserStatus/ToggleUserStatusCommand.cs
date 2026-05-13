namespace AWM.Service.Application.Features.Admin.Users.Commands.ToggleUserStatus;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to activate or deactivate a user account.
/// </summary>
public sealed record ToggleUserStatusCommand : IRequest<Result>
{
    public int UserId { get; init; }
    public bool IsActive { get; init; }
}

namespace AWM.Service.Application.Features.Auth.Auth.Commands.UpdateRoleOperationAction;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to update the permission matrix for a role.
/// Adds or removes RoleOperationAction entries.
/// </summary>
public sealed record UpdateRoleOperationActionCommand : IRequest<Result>
{
    public int RoleAccessId { get; init; }
    public int RoleOperationId { get; init; }
    public int RoleActionTypeId { get; init; }
    public bool IsGranted { get; init; }
}

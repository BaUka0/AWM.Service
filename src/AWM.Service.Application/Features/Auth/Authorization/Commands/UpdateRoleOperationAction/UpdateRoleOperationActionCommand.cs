namespace AWM.Service.Application.Features.Auth.Auth.Commands.UpdateRoleOperationAction;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record UpdateRoleOperationActionCommand : IRequest<Result>
{
    public int RoleAccessId { get; init; }
    public int RoleOperationId { get; init; }
    public int RoleActionTypeId { get; init; }
    public bool IsGranted { get; init; }
}

namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetRoleAccessMatrix;

using AWM.Service.Domain.Auth.ViewModels;
using MediatR;

/// <summary>
/// Query to get full permission matrix for a role.
/// </summary>
public sealed record GetRoleAccessMatrixQuery : IRequest<IReadOnlyList<RoleAccessMatrix>>
{
    public string RoleCode { get; init; } = null!;
}

namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetRoleAccessMatrix;

using AWM.Service.Domain.Auth.ViewModels;
using MediatR;

public sealed record GetRoleAccessMatrixQuery : IRequest<IReadOnlyList<RoleAccessMatrix>>
{
    public string RoleCode { get; init; } = null!;
}

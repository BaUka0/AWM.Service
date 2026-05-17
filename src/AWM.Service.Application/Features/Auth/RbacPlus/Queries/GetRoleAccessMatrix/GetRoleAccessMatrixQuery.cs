namespace AWM.Service.Application.Features.Auth.RbacPlus.Queries.GetRoleAccessMatrix;

using AWM.Service.Domain.Auth.RbacPlus.Repositories;
using AWM.Service.Domain.Auth.RbacPlus.ViewModels;
using MediatR;

/// <summary>
/// Query to get full permission matrix for a role.
/// </summary>
public sealed record GetRoleAccessMatrixQuery : IRequest<IReadOnlyList<RoleAccessMatrix>>
{
    public string RoleCode { get; init; } = null!;
}

public sealed class GetRoleAccessMatrixQueryHandler : IRequestHandler<GetRoleAccessMatrixQuery, IReadOnlyList<RoleAccessMatrix>>
{
    private readonly IRoleAccessRepository _roleAccessRepository;

    public GetRoleAccessMatrixQueryHandler(IRoleAccessRepository roleAccessRepository)
    {
        _roleAccessRepository = roleAccessRepository ?? throw new ArgumentNullException(nameof(roleAccessRepository));
    }

    public async Task<IReadOnlyList<RoleAccessMatrix>> Handle(GetRoleAccessMatrixQuery request, CancellationToken cancellationToken)
    {
        return await _roleAccessRepository.GetRoleAccessMatrixAsync(request.RoleCode, cancellationToken);
    }
}

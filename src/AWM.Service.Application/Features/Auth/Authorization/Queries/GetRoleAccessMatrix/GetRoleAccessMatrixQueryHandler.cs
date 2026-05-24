namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetRoleAccessMatrix;

using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Auth.ViewModels;
using MediatR;

/// <summary>
/// Handles retrieving the full permission matrix for a role.
/// </summary>
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

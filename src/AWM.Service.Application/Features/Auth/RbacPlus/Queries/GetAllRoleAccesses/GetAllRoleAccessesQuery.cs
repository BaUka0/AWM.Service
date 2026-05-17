namespace AWM.Service.Application.Features.Auth.RbacPlus.Queries.GetAllRoleAccesses;

using AWM.Service.Domain.Auth.RbacPlus.Entities;
using AWM.Service.Domain.Auth.RbacPlus.Repositories;
using MediatR;

/// <summary>
/// Query to get all role access definitions.
/// </summary>
public sealed record GetAllRoleAccessesQuery : IRequest<IReadOnlyList<RoleAccess>>;

public sealed class GetAllRoleAccessesQueryHandler : IRequestHandler<GetAllRoleAccessesQuery, IReadOnlyList<RoleAccess>>
{
    private readonly IRoleAccessRepository _roleAccessRepository;

    public GetAllRoleAccessesQueryHandler(IRoleAccessRepository roleAccessRepository)
    {
        _roleAccessRepository = roleAccessRepository ?? throw new ArgumentNullException(nameof(roleAccessRepository));
    }

    public async Task<IReadOnlyList<RoleAccess>> Handle(GetAllRoleAccessesQuery request, CancellationToken cancellationToken)
    {
        return await _roleAccessRepository.GetAllAsync(cancellationToken);
    }
}

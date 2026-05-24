namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetAllRoleAccesses;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using MediatR;

/// <summary>
/// Handles retrieving all role access definitions.
/// </summary>
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

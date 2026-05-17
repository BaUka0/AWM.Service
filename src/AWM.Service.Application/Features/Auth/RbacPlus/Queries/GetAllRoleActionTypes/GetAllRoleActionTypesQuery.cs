namespace AWM.Service.Application.Features.Auth.RbacPlus.Queries.GetAllRoleActionTypes;

using AWM.Service.Domain.Auth.RbacPlus.Entities;
using AWM.Service.Domain.Auth.RbacPlus.Repositories;
using MediatR;

/// <summary>
/// Query to get all role action types.
/// </summary>
public sealed record GetAllRoleActionTypesQuery : IRequest<IReadOnlyList<RoleActionType>>;

public sealed class GetAllRoleActionTypesQueryHandler : IRequestHandler<GetAllRoleActionTypesQuery, IReadOnlyList<RoleActionType>>
{
    private readonly IRoleActionTypeRepository _roleActionTypeRepository;

    public GetAllRoleActionTypesQueryHandler(IRoleActionTypeRepository roleActionTypeRepository)
    {
        _roleActionTypeRepository = roleActionTypeRepository ?? throw new ArgumentNullException(nameof(roleActionTypeRepository));
    }

    public async Task<IReadOnlyList<RoleActionType>> Handle(GetAllRoleActionTypesQuery request, CancellationToken cancellationToken)
    {
        return await _roleActionTypeRepository.GetAllAsync(cancellationToken);
    }
}

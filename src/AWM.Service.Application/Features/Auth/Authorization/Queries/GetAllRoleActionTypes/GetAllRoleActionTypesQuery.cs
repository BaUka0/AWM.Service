namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetAllRoleActionTypes;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
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

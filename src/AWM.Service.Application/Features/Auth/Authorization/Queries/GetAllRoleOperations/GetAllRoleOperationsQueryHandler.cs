namespace AWM.Service.Application.Features.Auth.Auth.Queries.GetAllRoleOperations;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using MediatR;

public sealed class GetAllRoleOperationsQueryHandler : IRequestHandler<GetAllRoleOperationsQuery, IReadOnlyList<RoleOperation>>
{
    private readonly IRoleOperationRepository _roleOperationRepository;

    public GetAllRoleOperationsQueryHandler(IRoleOperationRepository roleOperationRepository)
    {
        _roleOperationRepository = roleOperationRepository ?? throw new ArgumentNullException(nameof(roleOperationRepository));
    }

    public async Task<IReadOnlyList<RoleOperation>> Handle(GetAllRoleOperationsQuery request, CancellationToken cancellationToken)
    {
        return await _roleOperationRepository.GetTreeAsync(cancellationToken);
    }
}

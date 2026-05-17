namespace AWM.Service.Domain.Auth.RbacPlus.Repositories;

using AWM.Service.Domain.Auth.RbacPlus.Entities;

/// <summary>
/// Repository for RBAC+ role action types.
/// </summary>
public interface IRoleActionTypeRepository
{
    Task<RoleActionType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RoleActionType?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleActionType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(RoleActionType roleActionType, CancellationToken cancellationToken = default);
}

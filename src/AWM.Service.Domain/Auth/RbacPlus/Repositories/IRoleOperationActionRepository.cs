namespace AWM.Service.Domain.Auth.RbacPlus.Repositories;

using AWM.Service.Domain.Auth.RbacPlus.Entities;

/// <summary>
/// Repository for RBAC+ permission matrix (RoleOperationAction).
/// </summary>
public interface IRoleOperationActionRepository
{
    Task<RoleOperationAction?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleOperationAction>> GetByRoleAccessIdAsync(int roleAccessId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleOperationAction>> GetByRoleAccessIdAndOperationIdAsync(int roleAccessId, int roleOperationId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int roleAccessId, int roleOperationId, int roleActionTypeId, CancellationToken cancellationToken = default);
    Task AddAsync(RoleOperationAction roleOperationAction, CancellationToken cancellationToken = default);
    Task RemoveAsync(RoleOperationAction roleOperationAction, CancellationToken cancellationToken = default);
}

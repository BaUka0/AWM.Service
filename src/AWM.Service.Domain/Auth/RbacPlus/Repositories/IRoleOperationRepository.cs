namespace AWM.Service.Domain.Auth.RbacPlus.Repositories;

using AWM.Service.Domain.Auth.RbacPlus.Entities;

/// <summary>
/// Repository for RBAC+ role operations (modules).
/// </summary>
public interface IRoleOperationRepository
{
    Task<RoleOperation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RoleOperation?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleOperation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleOperation>> GetByParentIdAsync(int? parentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleOperation>> GetTreeAsync(CancellationToken cancellationToken = default);
    Task AddAsync(RoleOperation roleOperation, CancellationToken cancellationToken = default);
    Task UpdateAsync(RoleOperation roleOperation, CancellationToken cancellationToken = default);
}

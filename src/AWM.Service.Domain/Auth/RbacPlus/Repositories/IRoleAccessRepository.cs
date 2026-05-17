namespace AWM.Service.Domain.Auth.RbacPlus.Repositories;

using AWM.Service.Domain.Auth.RbacPlus.Entities;
using AWM.Service.Domain.Auth.RbacPlus.ViewModels;

/// <summary>
/// Repository for RBAC+ role access definitions.
/// </summary>
public interface IRoleAccessRepository
{
    Task<RoleAccess?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RoleAccess?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoleAccess>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(RoleAccess roleAccess, CancellationToken cancellationToken = default);
    Task UpdateAsync(RoleAccess roleAccess, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets full permission matrix for a role.
    /// </summary>
    Task<IReadOnlyList<RoleAccessMatrix>> GetRoleAccessMatrixAsync(string roleCode, CancellationToken cancellationToken = default);
}

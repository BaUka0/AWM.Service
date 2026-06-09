namespace AWM.Service.Domain.Auth.Repositories;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.ViewModels;

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

/// <summary>
/// Repository for RBAC+ role operations.
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

/// <summary>
/// Repository for RBAC+ role operation actions.
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

/// <summary>
/// Repository for RBAC+ user access.
/// </summary>
public interface IUserAccessRepository
{
    Task<UserAccess?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserAccess>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserAccess>> GetByRoleAccessIdAsync(int roleAccessId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserAccess>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int userId, int roleAccessId, CancellationToken cancellationToken = default);
    Task AddAsync(UserAccess userAccess, CancellationToken cancellationToken = default);
    Task RemoveAsync(UserAccess userAccess, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> CheckUserAccessAsync(int userId, string operationName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserAccessMatrix>> GetUserAccessMatrixAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReducedUserAccessMatrix>> GetReducedUserAccessMatrixAsync(int userId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for RBAC+ user access history.
/// </summary>
public interface IUserAccessHistoryRepository
{
    Task<IReadOnlyList<UserAccessHistory>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserAccessHistory>> GetByRoleAccessIdAsync(int roleAccessId, CancellationToken cancellationToken = default);
    Task AddAsync(UserAccessHistory history, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository for LocalAccount.
/// </summary>
public interface ILocalAccountRepository
{
    Task<LocalAccount?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<LocalAccount?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LocalAccount>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LocalAccount?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task AddAsync(LocalAccount localAccount, CancellationToken cancellationToken = default);
    Task UpdateAsync(LocalAccount localAccount, CancellationToken cancellationToken = default);
}

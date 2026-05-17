namespace AWM.Service.Domain.Auth.RbacPlus.Repositories;

using AWM.Service.Domain.Auth.RbacPlus.Entities;
using AWM.Service.Domain.Auth.RbacPlus.ViewModels;

/// <summary>
/// Repository for RBAC+ user access operations.
/// </summary>
public interface IUserAccessRepository
{
    Task<UserAccess?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserAccess>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserAccess>> GetByRoleAccessIdAsync(int roleAccessId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int userId, int roleAccessId, CancellationToken cancellationToken = default);
    Task AddAsync(UserAccess userAccess, CancellationToken cancellationToken = default);
    Task RemoveAsync(UserAccess userAccess, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks user access by querying the UserAccessMatrix view.
    /// Returns available action types for the given user and operation.
    /// </summary>
    Task<IReadOnlyList<string>> CheckUserAccessAsync(int userId, string operationName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets full permission matrix for a user.
    /// </summary>
    Task<IReadOnlyList<UserAccessMatrix>> GetUserAccessMatrixAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets reduced role list for a user.
    /// </summary>
    Task<IReadOnlyList<ReducedUserAccessMatrix>> GetReducedUserAccessMatrixAsync(int userId, CancellationToken cancellationToken = default);
}

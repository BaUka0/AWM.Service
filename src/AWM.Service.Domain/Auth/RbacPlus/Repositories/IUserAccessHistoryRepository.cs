namespace AWM.Service.Domain.Auth.RbacPlus.Repositories;

using AWM.Service.Domain.Auth.RbacPlus.Entities;

/// <summary>
/// Repository for user access history audit.
/// </summary>
public interface IUserAccessHistoryRepository
{
    Task<IReadOnlyList<UserAccessHistory>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserAccessHistory>> GetByRoleAccessIdAsync(int roleAccessId, CancellationToken cancellationToken = default);
    Task AddAsync(UserAccessHistory history, CancellationToken cancellationToken = default);
}

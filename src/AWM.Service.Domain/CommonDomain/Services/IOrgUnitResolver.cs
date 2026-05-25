namespace AWM.Service.Domain.CommonDomain.Services;

/// <summary>
/// Universal resolver for OrgUnitId. Supports resolution from Employee (main position)
/// and Student (speciality department) contexts.
/// </summary>
public interface IOrgUnitResolver
{
    /// <summary>
    /// Resolves the OrgUnitId for a user. If explicitOrgUnitId is provided, validates and returns it.
    /// Otherwise, attempts to determine OrgUnitId from the user's Employee or Student profile.
    /// </summary>
    /// <param name="explicitOrgUnitId">Optional explicit OrgUnitId provided by the caller.</param>
    /// <param name="userId">The user ID to resolve for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple of (Resolved OrgUnitId or null, error message if failed).</returns>
    Task<(int? OrgUnitId, string? ErrorMessage)> ResolveAsync(int? explicitOrgUnitId, int userId, CancellationToken cancellationToken = default);
}

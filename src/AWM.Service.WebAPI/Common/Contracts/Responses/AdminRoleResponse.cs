namespace AWM.Service.WebAPI.Common.Contracts.Responses;

/// <summary>
/// Response model for system roles with user counts.
/// </summary>
public sealed class AdminRoleResponse
{
    public int RoleId { get; init; }
    public string SystemName { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string ScopeLevel { get; init; } = string.Empty;
    public int UsersCount { get; init; }
}

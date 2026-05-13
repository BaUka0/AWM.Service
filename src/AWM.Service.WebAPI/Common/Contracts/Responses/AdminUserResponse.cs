namespace AWM.Service.WebAPI.Common.Contracts.Responses;

/// <summary>
/// Response model for admin user management.
/// </summary>
public sealed class AdminUserResponse
{
    public int UserId { get; init; }
    public string Login { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];
    public int? DepartmentId { get; init; }
    public string? DepartmentName { get; init; }
    public int? RoleId { get; init; }
    public DateTime CreatedAt { get; init; }
}

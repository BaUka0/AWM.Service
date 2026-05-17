namespace AWM.Service.Domain.Auth.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Legacy Role entity. Will be replaced by RoleAccess in RBAC+.
/// Kept for backward compatibility during migration.
/// </summary>
public class Role : Entity<int>, IAuditable
{
    public string SystemName { get; private set; } = null!;
    public string? DisplayName { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    private Role() { }

    public Role(string systemName, int createdBy = 0, string? displayName = null)
    {
        if (string.IsNullOrWhiteSpace(systemName))
            throw new ArgumentException("System name is required.", nameof(systemName));

        SystemName = systemName;
        DisplayName = displayName ?? systemName;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Creates a role with display name.
    /// </summary>
    public static Role Create(string systemName, string displayName, int createdBy = 0)
    {
        return new Role(systemName, createdBy, displayName);
    }
}

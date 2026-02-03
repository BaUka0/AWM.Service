namespace AWM.Service.Domain.Auth.Entities;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Auth.Enums;

/// <summary>
/// Role entity defining system roles for RBAC.
/// </summary>
public class Role : Entity<int>, IAuditable
{
    public string SystemName { get; private set; } = null!;
    public string? DisplayName { get; private set; }
    public ScopeLevel ScopeLevel { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public int CreatedBy { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public int? LastModifiedBy { get; private set; }

    private Role() { }

    public Role(string systemName, ScopeLevel scopeLevel, int createdBy = 0, string? displayName = null)
    {
        if (string.IsNullOrWhiteSpace(systemName))
            throw new ArgumentException("System name is required.", nameof(systemName));

        SystemName = systemName;
        ScopeLevel = scopeLevel;
        DisplayName = displayName ?? systemName;
        
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    /// <summary>
    /// Creates a role from the RoleType enum.
    /// </summary>
    public static Role FromRoleType(RoleType roleType, ScopeLevel scopeLevel, int createdBy = 0)
    {
        return new Role(roleType.ToString(), scopeLevel, createdBy, GetDisplayName(roleType));
    }

    private static string GetDisplayName(RoleType roleType)
    {
        return roleType switch
        {
            RoleType.Student => "Студент",
            RoleType.Supervisor => "Научный руководитель",
            RoleType.HeadOfDepartment => "Заведующий кафедрой",
            RoleType.Secretary => "Секретарь",
            RoleType.Expert => "Эксперт",
            RoleType.Admin => "Администратор",
            RoleType.CommissionMember => "Член комиссии",
            RoleType.ViceRector => "Проректор по УР",
            _ => roleType.ToString()
        };
    }
}

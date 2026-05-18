namespace AWM.Service.Domain.Defense.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Commission role reference entity. Replaces RoleInCommission enum.
/// Maps to [Defense].[CommissionRoles].
/// </summary>
public class CommissionRole : Entity<int>
{
    public string Title { get; private set; } = null!;

    private CommissionRole() { }

    public CommissionRole(int id, string title)
    {
        Id = id;
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        Title = title;
    }
}

namespace AWM.Service.Domain.Defense.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Commission type reference entity.
/// Maps to [Defense].[CommissionTypes].
/// </summary>
public class CommissionType : Entity<int>
{
    public string Title { get; private set; } = null!;

    private CommissionType() { }

    public CommissionType(int id, string title)
    {
        Id = id;
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("CommissionType.TitleRequired", "Title is required.");

        Title = title;
    }
}

namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Check type reference entity. Replaces both CheckType and ExpertiseType enums.
/// Maps to [Thesis].[CheckTypes].
/// </summary>
public class CheckType : Entity<int>
{
    public string Title { get; private set; } = null!;

    private CheckType() { }

    public CheckType(int id, string title)
    {
        Id = id;
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("CheckType.TitleRequired", "Title is required.");

        Title = title;
    }
}

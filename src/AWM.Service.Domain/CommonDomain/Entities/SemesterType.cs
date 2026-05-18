namespace AWM.Service.Domain.CommonDomain.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Semester type reference entity (e.g., Fall, Spring, Summer, Winter).
/// Maps to [Edu].[SemesterTypes].
/// </summary>
public class SemesterType : Entity<int>
{
    public string Title { get; private set; } = null!;
    public int OrderBy { get; private set; }

    private SemesterType() { }

    public SemesterType(string title, int orderBy)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        Title = title;
        OrderBy = orderBy;
    }
}

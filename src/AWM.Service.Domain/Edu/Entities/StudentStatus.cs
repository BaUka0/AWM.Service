namespace AWM.Service.Domain.Edu.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Student status reference entity.
/// Maps to [Edu].[StudentStatuses].
/// </summary>
public class StudentStatus : Entity<int>
{
    public string Title { get; private set; } = null!;

    private StudentStatus() { }

    public StudentStatus(int id, string title)
    {
        Id = id;
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        Title = title;
    }
}

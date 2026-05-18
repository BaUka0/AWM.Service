namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Application status reference entity.
/// Maps to [Thesis].[ApplicationStatuses].
/// </summary>
public class ApplicationStatus : Entity<int>
{
    public string Title { get; private set; } = null!;

    private ApplicationStatus() { }

    public ApplicationStatus(int id, string title)
    {
        Id = id;
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        Title = title;
    }
}

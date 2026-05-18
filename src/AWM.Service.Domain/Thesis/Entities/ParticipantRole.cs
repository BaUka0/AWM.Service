namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Participant role reference entity.
/// Maps to [Thesis].[ParticipantRoles].
/// </summary>
public class ParticipantRole : Entity<int>
{
    public string Title { get; private set; } = null!;

    private ParticipantRole() { }

    public ParticipantRole(int id, string title)
    {
        Id = id;
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        Title = title;
    }
}

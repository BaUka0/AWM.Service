namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Attachment type reference entity.
/// Maps to [Thesis].[AttachmentTypes].
/// </summary>
public class AttachmentType : Entity<int>
{
    public string Title { get; private set; } = null!;

    private AttachmentType() { }

    public AttachmentType(int id, string title)
    {
        Id = id;
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));

        Title = title;
    }
}

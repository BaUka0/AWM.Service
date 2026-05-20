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
            throw new DomainException("AttachmentType.TitleRequired", "Title is required.");

        Title = title;
    }
}

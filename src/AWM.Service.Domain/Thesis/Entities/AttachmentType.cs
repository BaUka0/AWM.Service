namespace AWM.Service.Domain.Thesis.Entities;

using AWM.Service.Domain.Common;

/// <summary>
/// Attachment type reference entity.
/// Defines types of attachments (e.g., Draft, Final Work, Report).
/// Maps to [Thesis].[AttachmentTypes].
/// </summary>
public class AttachmentType : Entity<int>
{
    public string Title { get; private set; } = null!;
    
    /// <summary>
    /// Optional system code for hardcoded logic (e.g., "FINAL_WORK").
    /// </summary>
    public string? Code { get; private set; }

    private AttachmentType() { }

    public AttachmentType(int id, string title, string? code = null)
    {
        Id = id;
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("AttachmentType.TitleRequired", "Title is required.");

        Title = title;
        Code = code;
    }

    public void Update(string title, string? code)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("AttachmentType.TitleRequired", "Title is required.");

        Title = title;
        Code = code;
    }
}

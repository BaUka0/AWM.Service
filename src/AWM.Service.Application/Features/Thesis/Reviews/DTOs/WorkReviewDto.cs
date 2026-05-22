namespace AWM.Service.Application.Features.Thesis.Reviews.DTOs;

using AWM.Service.Domain.Thesis.Enums;

public class WorkReviewDto
{
    public long Id { get; set; }
    public long WorkId { get; set; }
    public int AuthorUserId { get; set; }
    public ReviewType Type { get; set; }
    public string ReviewText { get; set; } = null!;
    public string? MetadataJson { get; set; }
    public bool IsFinal { get; set; }
    
    // Attachments will be loaded separately or included here later if needed
    
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public int? LastModifiedBy { get; set; }
}

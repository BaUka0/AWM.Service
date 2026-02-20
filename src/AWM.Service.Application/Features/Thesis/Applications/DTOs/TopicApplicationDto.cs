namespace AWM.Service.Application.Features.Thesis.Applications.DTOs;

using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;

/// <summary>
/// Data transfer object for topic applications.
/// </summary>
public sealed record TopicApplicationDto
{
    /// <summary>
    /// Application ID.
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Topic ID.
    /// </summary>
    public long TopicId { get; init; }

    /// <summary>
    /// Student ID.
    /// </summary>
    public int StudentId { get; init; }

    /// <summary>
    /// Optional motivation letter.
    /// </summary>
    public string? MotivationLetter { get; init; }

    /// <summary>
    /// Date when application was submitted.
    /// </summary>
    public DateTime AppliedAt { get; init; }

    /// <summary>
    /// Current status of the application.
    /// </summary>
    public ApplicationStatus Status { get; init; }

    /// <summary>
    /// Status as string for easier display.
    /// </summary>
    public string StatusText { get; init; } = null!;

    /// <summary>
    /// Date when application was reviewed (if reviewed).
    /// </summary>
    public DateTime? ReviewedAt { get; init; }

    /// <summary>
    /// ID of the user who reviewed (if reviewed).
    /// </summary>
    public int? ReviewedBy { get; init; }

    /// <summary>
    /// Review comment (rejection reason or acceptance note).
    /// </summary>
    public string? ReviewComment { get; init; }

    /// <summary>
    /// Indicates if application is pending review.
    /// </summary>
    public bool IsPending { get; init; }

    /// <summary>
    /// Indicates if application was accepted.
    /// </summary>
    public bool IsAccepted { get; init; }

    /// <summary>
    /// Indicates if application is deleted (withdrawn).
    /// </summary>
    public bool IsDeleted { get; init; }

    /// <summary>
    /// Maps domain entity to DTO.
    /// </summary>
    public static TopicApplicationDto FromEntity(TopicApplication entity)
    {
        return new TopicApplicationDto
        {
            Id = entity.Id,
            TopicId = entity.TopicId,
            StudentId = entity.StudentId,
            MotivationLetter = entity.MotivationLetter,
            AppliedAt = entity.AppliedAt,
            Status = entity.Status,
            StatusText = entity.Status.ToString(),
            ReviewedAt = entity.ReviewedAt,
            ReviewedBy = entity.ReviewedBy,
            ReviewComment = entity.ReviewComment,
            IsPending = entity.IsPending,
            IsAccepted = entity.IsAccepted,
            IsDeleted = entity.IsDeleted
        };
    }
}
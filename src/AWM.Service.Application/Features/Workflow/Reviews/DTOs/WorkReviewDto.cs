using AWM.Service.Domain.Thesis.Enums;
using System;

namespace AWM.Service.Application.Features.Workflow.Reviews.DTOs;

public record WorkReviewDto(
    long Id,
    long WorkId,
    int AuthorUserId,
    string AuthorName,
    ReviewType Type,
    string ReviewText,
    string? MetadataJson,
    bool IsFinal,
    DateTime CreatedAt);

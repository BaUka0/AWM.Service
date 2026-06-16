namespace AWM.Service.Application.Features.Workflow.Reviewers.DTOs;

public record ReviewerDto(
    int Id,
    string FullName,
    string? Position,
    string? AcademicDegree,
    string? Organization,
    string? Email,
    string? Phone,
    bool IsActive,
    int? UserId);

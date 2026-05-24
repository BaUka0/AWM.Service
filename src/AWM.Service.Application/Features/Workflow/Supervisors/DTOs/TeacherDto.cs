namespace AWM.Service.Application.Features.Workflow.Supervisors.DTOs;

public record TeacherDto(
    int UserId,
    string FullName,
    string? Email,
    string PositionTitle,
    int? MaxWorkload = null
);

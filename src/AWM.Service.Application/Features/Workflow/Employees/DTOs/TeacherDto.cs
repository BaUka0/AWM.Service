namespace AWM.Service.Application.Features.Workflow.Employees.DTOs;

public record TeacherDto(
    int UserId,
    string FullName,
    string? Email,
    string PositionTitle,
    int? MaxWorkload = null,
    int CurrentStudents = 0
);

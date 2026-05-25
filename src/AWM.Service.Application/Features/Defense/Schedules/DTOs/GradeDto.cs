namespace AWM.Service.Application.Features.Defense.Schedules.DTOs;

public record GradeDto(
    long Id,
    long ScheduleId,
    long AssignmentId,
    int CriteriaId,
    int Score,
    string? Comment,
    string MemberName
);

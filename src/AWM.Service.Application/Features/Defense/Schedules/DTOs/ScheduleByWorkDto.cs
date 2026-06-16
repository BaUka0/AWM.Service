namespace AWM.Service.Application.Features.Defense.Schedules.DTOs;

public record ScheduleByWorkDto(
    long? ScheduleId,
    string? DefenseDate,
    string? DefenseTime,
    string? Location,
    int? CommissionId,
    string? CommissionName,
    IReadOnlyList<CommissionMemberInfoDto>? Members,
    bool IsReconciliationStarted,
    decimal? AverageScore);

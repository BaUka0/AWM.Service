namespace AWM.Service.Application.Features.Defense.Schedules.DTOs;

public record DefenseStepDto(
    string StepType,
    int? AttemptNumber,
    ScheduleInfoDto? Schedule,
    IReadOnlyList<CommissionMemberInfoDto> Commission,
    IReadOnlyList<AttemptHistoryDto> PreviousAttempts,
    DefenseResultsDto? Results
);

public record ScheduleInfoDto(
    string Date,
    string Time,
    string Location
);

public record CommissionMemberInfoDto(
    string Role,
    string Name
);

public record AttemptHistoryDto(
    int AttemptNumber,
    bool IsPassed,
    decimal Score,
    string Date,
    string? Comments
);

public record DefenseResultsDto(
    bool IsPassed,
    decimal Score,
    string GradeLetter,
    string Decision,
    string Date
);

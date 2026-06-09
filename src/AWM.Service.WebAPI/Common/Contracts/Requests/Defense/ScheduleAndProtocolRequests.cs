namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

public record AddGradeRequest(
    int CriteriaId,
    int Score,
    string? Comment = null
);

public record CreateProtocolRequest(
    long ScheduleId,
    string? ProtocolNumber = null,
    decimal? FinalScoreNumeric = null,
    string? FinalGradeLetter = null,
    string? Decision = null,
    string? Comments = null,
    int? DecisionType = null,
    int? ReadinessPercent = null
);

public record FinalizeProtocolRequest(bool IsStudentPresent = true);

public record NotifyUnreadyStudentsRequest(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId = null
);

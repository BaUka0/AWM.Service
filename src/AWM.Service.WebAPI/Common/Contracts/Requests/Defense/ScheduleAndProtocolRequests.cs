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
    string? Comments = null
);

namespace AWM.Service.WebAPI.Common.Contracts.Responses.Thesis;

/// <summary>
/// Response contract for student defense step (pre-defense or final defense).
/// </summary>
public sealed record StudentDefenseStepResponse
{
    public string StepType { get; init; } = null!;
    public int? AttemptNumber { get; init; }

    public DefenseStepScheduleResponse? Schedule { get; init; }
    public IReadOnlyList<DefenseStepMemberResponse> Commission { get; init; } = [];
    public IReadOnlyList<DefenseStepAttemptResponse> PreviousAttempts { get; init; } = [];
    public DefenseStepResultsResponse? Results { get; init; }
}

public sealed record DefenseStepScheduleResponse
{
    public DateTime Date { get; init; }
    public string? Time { get; init; }
    public string? Location { get; init; }
}

public sealed record DefenseStepMemberResponse
{
    public string Name { get; init; } = null!;
    public string Role { get; init; } = null!;
}

public sealed record DefenseStepAttemptResponse
{
    public int AttemptNumber { get; init; }
    public DateTime Date { get; init; }
    public decimal? Score { get; init; }
    public bool IsPassed { get; init; }
    public string? Comments { get; init; }
}

public sealed record DefenseStepResultsResponse
{
    public string? FinalGrade { get; init; }
    public decimal? CommissionGrade { get; init; }
    public decimal? FinalScore { get; init; }
    public int? Readiness { get; init; }
    public string? Comments { get; init; }
}

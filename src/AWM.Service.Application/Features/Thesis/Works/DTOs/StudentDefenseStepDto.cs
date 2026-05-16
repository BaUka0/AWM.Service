namespace AWM.Service.Application.Features.Thesis.Works.DTOs;

public sealed record StudentDefenseStepDto
{
    public string StepType { get; init; } = null!;
    public int? AttemptNumber { get; init; }

    public DefenseStepScheduleDto? Schedule { get; init; }
    public IReadOnlyList<DefenseStepMemberDto> Commission { get; init; } = [];
    public IReadOnlyList<DefenseStepAttemptDto> PreviousAttempts { get; init; } = [];
    public DefenseStepResultsDto? Results { get; init; }
}

public sealed record DefenseStepScheduleDto
{
    public DateTime Date { get; init; }
    public string? Time { get; init; }
    public string? Location { get; init; }
}

public sealed record DefenseStepMemberDto
{
    public string Name { get; init; } = null!;
    public string Role { get; init; } = null!;
}

public sealed record DefenseStepAttemptDto
{
    public int AttemptNumber { get; init; }
    public DateTime Date { get; init; }
    public decimal? Score { get; init; }
    public bool IsPassed { get; init; }
    public string? Comments { get; init; }
}

public sealed record DefenseStepResultsDto
{
    public string? FinalGrade { get; init; }
    public decimal? CommissionGrade { get; init; }
    public decimal? FinalScore { get; init; }
    public int? Readiness { get; init; }
    public string? Comments { get; init; }
}

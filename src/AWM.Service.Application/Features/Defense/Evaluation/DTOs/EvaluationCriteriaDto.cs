namespace AWM.Service.Application.Features.Defense.Evaluation.DTOs;

/// <summary>
/// DTO representing evaluation criteria for defense grading.
/// </summary>
public sealed record EvaluationCriteriaDto
{
    /// <summary>Criteria ID.</summary>
    public int Id { get; init; }

    /// <summary>Work type ID this criteria applies to.</summary>
    public int WorkTypeId { get; init; }

    /// <summary>Org unit ID. Null for university-wide criteria.</summary>
    public int? OrgUnitId { get; init; }

    /// <summary>Speciality ID. Null for department-level or university-wide criteria.</summary>
    public int? SpecialityId { get; init; }

    /// <summary>Name of the criteria.</summary>
    public string CriteriaName { get; init; } = null!;

    /// <summary>Maximum score for this criteria.</summary>
    public int MaxScore { get; init; }

    /// <summary>Weight of this criteria in the final grade calculation.</summary>
    public decimal Weight { get; init; }
}

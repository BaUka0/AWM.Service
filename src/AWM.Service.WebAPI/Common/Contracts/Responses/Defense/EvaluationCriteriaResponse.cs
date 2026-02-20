namespace AWM.Service.WebAPI.Common.Contracts.Responses.Defense;

/// <summary>
/// Response contract for evaluation criteria.
/// </summary>
public sealed record EvaluationCriteriaResponse
{
    /// <summary>Criteria ID.</summary>
    /// <example>3</example>
    public int Id { get; init; }

    /// <summary>Work type ID this criteria applies to.</summary>
    /// <example>1</example>
    public int WorkTypeId { get; init; }

    /// <summary>Department ID. Null for university-wide criteria.</summary>
    /// <example>2</example>
    public int? DepartmentId { get; init; }

    /// <summary>Name of the criteria.</summary>
    /// <example>Качество оформления</example>
    public string CriteriaName { get; init; } = null!;

    /// <summary>Maximum score for this criteria.</summary>
    /// <example>100</example>
    public int MaxScore { get; init; }

    /// <summary>Weight of this criteria in the final grade calculation.</summary>
    /// <example>1.5</example>
    public decimal Weight { get; init; }
}

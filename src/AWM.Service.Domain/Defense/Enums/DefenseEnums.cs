namespace AWM.Service.Domain.Defense.Enums;

public enum PreDefenseStatus
{
    Attended = 1,
    Absent = 2,
    Excused = 3
}

public enum CommissionTypes
{
    PreDefense = 1,
    GAK = 2
}

/// <summary>
/// Differentiates evaluation criteria sets for different defense stages.
/// Used in EvaluationCriteria.DefenseStageType.
/// </summary>
public static class DefenseStageTypes
{
    /// <summary>Pre-defense criteria (ПЗ-1/2/3). Default 8 criteria, max 100 points.</summary>
    public const int PreDefense = 1;

    /// <summary>GAK (final defense) criteria. Department-configurable set.</summary>
    public const int GAK = 2;
}

/// <summary>
/// Structured decision types for defense protocols.
/// Replaces free-text Decision comparison in FinalizeProtocolCommandHandler.
/// </summary>
public static class ProtocolDecisionTypes
{
    /// <summary>Допущен к защите / Passed</summary>
    public const int Admitted = 1;

    /// <summary>Не допущен / Not admitted</summary>
    public const int NotAdmitted = 2;

    /// <summary>Доработать / Needs revision</summary>
    public const int NeedsRevision = 3;
}

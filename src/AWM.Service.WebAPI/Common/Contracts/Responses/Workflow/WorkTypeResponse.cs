namespace AWM.Service.WebAPI.Common.Contracts.Responses.Workflow;

/// <summary>
/// Response model for a single work type dictionary item.
/// </summary>
public sealed class WorkTypeResponse
{
    public int Id { get; init; }

    /// <summary>
    /// System name: "CourseWork", "DiplomaWork", "MasterThesis", "PhD".
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Associated degree level ID (null for CourseWork).
    /// </summary>
    public int? DegreeLevelId { get; init; }
}

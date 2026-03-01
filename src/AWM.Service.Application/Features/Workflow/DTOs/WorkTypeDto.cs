namespace AWM.Service.Application.Features.Workflow.DTOs;

/// <summary>
/// DTO for a work type dictionary item.
/// Used to build dynamic dropdowns on the frontend.
/// </summary>
public sealed class WorkTypeDto
{
    public int Id { get; init; }

    /// <summary>
    /// System name: "CourseWork", "DiplomaWork", "MasterThesis", "PhD".
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Associated degree level ID (null for CourseWork which is not degree-specific).
    /// </summary>
    public int? DegreeLevelId { get; init; }
}

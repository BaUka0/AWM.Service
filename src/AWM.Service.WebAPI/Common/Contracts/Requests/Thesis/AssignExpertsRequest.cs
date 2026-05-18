namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

using System.Collections.Generic;

/// <summary>
/// Request contract for assigning experts to quality check types.
/// </summary>
public sealed record AssignExpertsRequest
{
    /// <summary>
    /// Department ID.
    /// </summary>
    /// <example>1</example>
    public int DepartmentId { get; init; }

    /// <summary>
    /// List of expert assignments.
    /// </summary>
    public IReadOnlyList<ExpertAssignmentItem> Assignments { get; init; } = new List<ExpertAssignmentItem>();
}

public sealed record ExpertAssignmentItem
{
    /// <summary>
    /// User ID of the expert.
    /// </summary>
    /// <example>42</example>
    public int UserId { get; init; }

    /// <summary>
    /// Type of expertise (1=NormControl, 2=SoftwareCheck, 3=AntiPlagiarism).
    /// </summary>
    /// <example>1</example>
    public int ExpertiseType { get; init; }
}

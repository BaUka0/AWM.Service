namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

using System.Collections.Generic;
using AWM.Service.Domain.Thesis.Enums;

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
    /// Type of expertise (0=NormControl, 1=SoftwareCheck, 2=AntiPlagiarism).
    /// </summary>
    /// <example>0</example>
    public ExpertiseType ExpertiseType { get; init; }
}

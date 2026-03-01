namespace AWM.Service.WebAPI.Common.Contracts.Requests.Edu;

/// <summary>
/// Request to update staff workload limits.
/// </summary>
public sealed record UpdateStaffWorkloadRequest
{
    /// <summary>
    /// Maximum number of students the staff member can supervise.
    /// </summary>
    public int MaxStudentsLoad { get; init; }
}

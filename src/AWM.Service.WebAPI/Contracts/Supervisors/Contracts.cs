namespace AWM.Service.WebAPI.Contracts.Supervisors;

/// <summary>
/// Request to assign a specific teacher as a supervisor with a workload limit.
/// </summary>
public record SupervisorAssignmentRequest(int UserId, int MaxWorkload);

/// <summary>
/// Request to approve a list of supervisors for a department/semester/speciality.
/// </summary>
public record ApproveSupervisorsRequest(
    int SemesterId,
    int? SpecialityId,
    List<SupervisorAssignmentRequest> Assignments
);

/// <summary>
/// Request to update the workload limit for an existing supervisor.
/// </summary>
public record UpdateSupervisorWorkloadRequest(int MaxWorkload);

/// <summary>
/// Response containing teacher information and their supervisor workload.
/// </summary>
public record TeacherResponse(
    int UserId,
    string FullName,
    string? Email,
    string PositionTitle,
    int? MaxWorkload
);

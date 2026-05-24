namespace AWM.Service.Application.Features.Workflow.Supervisors.DTOs;

/// <summary>
/// Metadata JSON structure for Supervisor assignments.
/// </summary>
public class SupervisorAssignmentMetadata
{
    public int SemesterId { get; set; }
    public int? SpecialityId { get; set; }
    public int? MaxWorkload { get; set; }
}

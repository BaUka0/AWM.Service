namespace AWM.Service.Application.Features.Workflow.Employees.DTOs;

public class EmployeeAssignmentMetadata
{
    public int SemesterId { get; set; }
    public int? SpecialityId { get; set; }
    public int? MaxWorkload { get; set; }
    public bool IsConfirmed { get; set; }
}

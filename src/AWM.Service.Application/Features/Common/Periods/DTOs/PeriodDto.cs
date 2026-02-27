namespace AWM.Service.Application.Features.Common.Periods.DTOs;

public sealed record PeriodDto
{
    public int Id { get; init; }
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
    public string WorkflowStage { get; init; } = null!;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsActive { get; init; }
    public bool IsCurrentlyOpen { get; init; }
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }
}

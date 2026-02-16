namespace AWM.Service.WebAPI.Common.Contracts.Responses.Common;

public record PeriodResponse
{
    public int Id { get; init; }
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
    public string WorkflowStage { get; init; } = null!;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsActive { get; init; }
    public bool IsCurrentlyOpen { get; init; }
}

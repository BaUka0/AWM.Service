namespace AWM.Service.WebAPI.Common.Contracts.Requests.Common;

using AWM.Service.Domain.CommonDomain.Enums;

public record CreatePeriodRequest
{
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
    public WorkflowStage WorkflowStage { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

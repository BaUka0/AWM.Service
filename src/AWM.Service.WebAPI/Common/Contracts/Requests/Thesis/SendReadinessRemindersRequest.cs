namespace AWM.Service.WebAPI.Common.Contracts.Requests.Thesis;

public sealed record SendReadinessRemindersRequest
{
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
}

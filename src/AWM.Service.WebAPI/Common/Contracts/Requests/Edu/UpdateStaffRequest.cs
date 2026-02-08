namespace AWM.Service.WebAPI.Common.Contracts.Requests.Edu;

public record UpdateStaffRequest
{
    public string? Position { get; init; }
    public string? AcademicDegree { get; init; }
    public int? MaxStudentsLoad { get; init; }
    public int? DepartmentId { get; init; }
}

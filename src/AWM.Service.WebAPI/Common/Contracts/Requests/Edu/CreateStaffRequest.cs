namespace AWM.Service.WebAPI.Common.Contracts.Requests.Edu;

public record CreateStaffRequest
{
    public int UserId { get; init; }
    public int DepartmentId { get; init; }
    public string? Position { get; init; }
    public string? AcademicDegree { get; init; }
    public int MaxStudentsLoad { get; init; } = 5;
}

namespace AWM.Service.WebAPI.Common.Contracts.Responses.Edu;

public record StaffResponse
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string? FullName { get; init; }
    public string? Email { get; init; }
    public string? Position { get; init; }
    public string? AcademicDegree { get; init; }
    public int DepartmentId { get; init; }
    public string? DepartmentName { get; init; }
    public int MaxStudentsLoad { get; init; }
}

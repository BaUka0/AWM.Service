namespace AWM.Service.Application.Features.Edu.Staff.DTOs;

public sealed record StaffDto
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
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }
}

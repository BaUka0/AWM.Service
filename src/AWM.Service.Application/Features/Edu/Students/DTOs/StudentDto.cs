namespace AWM.Service.Application.Features.Edu.Students.DTOs;

public sealed record StudentDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string? FullName { get; init; }
    public string? Email { get; init; }
    public string? GroupCode { get; init; }
    public int ProgramId { get; init; }
    public string? ProgramName { get; init; }
    public int AdmissionYear { get; init; }
    public int CurrentCourse { get; init; }
    public string Status { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }
}

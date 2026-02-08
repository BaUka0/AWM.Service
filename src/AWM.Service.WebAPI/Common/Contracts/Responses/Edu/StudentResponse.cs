namespace AWM.Service.WebAPI.Common.Contracts.Responses.Edu;

public record StudentResponse
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
}

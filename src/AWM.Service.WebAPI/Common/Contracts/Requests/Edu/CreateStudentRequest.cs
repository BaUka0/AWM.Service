namespace AWM.Service.WebAPI.Common.Contracts.Requests.Edu;

public record CreateStudentRequest
{
    public int UserId { get; init; }
    public int ProgramId { get; init; }
    public int AdmissionYear { get; init; }
    public int CurrentCourse { get; init; }
    public string? GroupCode { get; init; }
}

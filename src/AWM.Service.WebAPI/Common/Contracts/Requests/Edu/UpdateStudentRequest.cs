namespace AWM.Service.WebAPI.Common.Contracts.Requests.Edu;

public record UpdateStudentRequest
{
    public int? ProgramId { get; init; }
    public string? GroupCode { get; init; }
    public int? CurrentCourse { get; init; }
}

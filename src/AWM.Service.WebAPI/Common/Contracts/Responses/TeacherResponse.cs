namespace AWM.Service.WebAPI.Common.Contracts.Responses;

public record TeacherResponse(
    int UserId,
    string FullName,
    string? Email,
    string PositionTitle,
    int? MaxWorkload
);

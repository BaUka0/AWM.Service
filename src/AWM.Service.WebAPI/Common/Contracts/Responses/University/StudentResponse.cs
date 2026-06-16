namespace AWM.Service.WebAPI.Common.Contracts.Responses.University;

public record StudentResponse(
    int Id,
    string FullName,
    string Group,
    string Program,
    int Year,
    string Status
);

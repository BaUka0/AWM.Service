namespace AWM.Service.WebAPI.Common.Contracts.Responses.University;

public record StudentResponse(
    int Id,
    string Iin,
    string FullName,
    string Email,
    int Course,
    int SpecialityId,
    string SpecialityCode,
    string SpecialityName,
    int? DegreeLevelId
);

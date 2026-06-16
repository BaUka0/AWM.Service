namespace AWM.Service.WebAPI.Common.Contracts.Responses.University;

public record SpecialityLevelResponse(
    int Id,
    string NameRu,
    string NameKz,
    string NameEn,
    string Name
);

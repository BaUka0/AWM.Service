namespace AWM.Service.WebAPI.Common.Contracts.Responses.University;

public record DegreeLevelResponse(
    int Id,
    string NameRu,
    string NameKz,
    string NameEn
);

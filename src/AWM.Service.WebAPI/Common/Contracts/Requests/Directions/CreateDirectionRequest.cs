namespace AWM.Service.WebAPI.Common.Contracts.Requests.Directions;

public record CreateDirectionRequest(
    int SemesterId,
    int WorkTypeId,
    string TitleRu,
    string? TitleKz,
    string? TitleEn,
    string? DescriptionRu,
    string? DescriptionKz,
    string? DescriptionEn);

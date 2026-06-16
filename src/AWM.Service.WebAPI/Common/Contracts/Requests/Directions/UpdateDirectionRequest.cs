namespace AWM.Service.WebAPI.Common.Contracts.Requests.Directions;

public record UpdateDirectionRequest(
    string TitleRu,
    string? TitleKz,
    string? TitleEn,
    string? DescriptionRu,
    string? DescriptionKz,
    string? DescriptionEn);

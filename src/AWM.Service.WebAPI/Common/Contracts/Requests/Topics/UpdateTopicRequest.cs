namespace AWM.Service.WebAPI.Common.Contracts.Requests.Topics;

public record UpdateTopicRequest(
    string TitleRu,
    string? TitleKz = null,
    string? TitleEn = null,
    string? DescriptionRu = null,
    string? DescriptionKz = null,
    string? DescriptionEn = null,
    int? MaxParticipants = null);

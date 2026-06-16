namespace AWM.Service.WebAPI.Common.Contracts.Requests.Topics;

public record CreateTopicRequest(
    int SemesterId,
    int WorkTypeId,
    string TitleRu,
    long? DirectionId = null,
    string? TitleKz = null,
    string? TitleEn = null,
    string? DescriptionRu = null,
    string? DescriptionKz = null,
    string? DescriptionEn = null,
    int MaxParticipants = 1,
    int? SpecialityId = null,
    int? OrgUnitId = null);

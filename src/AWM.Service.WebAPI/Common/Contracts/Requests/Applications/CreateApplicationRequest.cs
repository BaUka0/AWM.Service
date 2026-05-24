namespace AWM.Service.WebAPI.Common.Contracts.Requests.Applications;

public record CreateApplicationRequest(
    long TopicId,
    string? MotivationLetter = null);

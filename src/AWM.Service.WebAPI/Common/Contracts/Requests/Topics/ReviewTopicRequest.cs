namespace AWM.Service.WebAPI.Common.Contracts.Requests.Topics;

public record ReviewTopicRequest(bool IsApproved, string? Comment);

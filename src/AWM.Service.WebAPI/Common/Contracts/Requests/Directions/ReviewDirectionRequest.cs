namespace AWM.Service.WebAPI.Common.Contracts.Requests.Directions;

public record ReviewDirectionRequest(
    int DecisionId,
    string? Comment);

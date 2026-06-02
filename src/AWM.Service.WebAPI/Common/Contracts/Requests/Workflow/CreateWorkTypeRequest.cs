namespace AWM.Service.WebAPI.Common.Contracts.Requests.Workflow;

public record CreateWorkTypeRequest(
    string Name,
    string Description,
    int DurationDays
);

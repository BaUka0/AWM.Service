namespace AWM.Service.WebAPI.Common.Contracts.Requests.Workflow;

public record UpdateWorkTypeRequest(
    string Name,
    string Description,
    int DurationDays,
    bool IsActive
);

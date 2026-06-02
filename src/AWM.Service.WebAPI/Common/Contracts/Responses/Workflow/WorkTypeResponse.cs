namespace AWM.Service.WebAPI.Common.Contracts.Responses.Workflow;

public record WorkTypeResponse(
    int Id,
    string Name,
    string Description,
    int DurationDays,
    bool IsActive
);

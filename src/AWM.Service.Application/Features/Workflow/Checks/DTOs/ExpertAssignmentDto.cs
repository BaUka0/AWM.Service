namespace AWM.Service.Application.Features.Workflow.Checks.DTOs;

public record ExpertAssignmentDto(
    long Id,
    int UserId,
    string UserFullName,
    int CheckTypeId,
    string CheckTypeName,
    bool IsActive);

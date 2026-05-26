namespace AWM.Service.Application.Features.Workflow.Checks.DTOs;

public record ExpertAssignmentInput(int UserId, int CheckTypeId, bool IsActive);

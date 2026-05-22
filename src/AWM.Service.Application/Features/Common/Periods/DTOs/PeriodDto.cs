namespace AWM.Service.Application.Features.Common.Stages.DTOs;

public sealed record StageDto
{
    public int Id { get; init; }
    public int OrgUnitId { get; init; }
    public int? SpecialityId { get; init; }
    public int SemesterId { get; init; }
    public int WorkflowStageId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public bool IsActive { get; init; }
    public bool IsCurrentlyOpen { get; init; }
    public DateTime CreatedAt { get; init; }
    public int CreatedBy { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public int? LastModifiedBy { get; init; }
}

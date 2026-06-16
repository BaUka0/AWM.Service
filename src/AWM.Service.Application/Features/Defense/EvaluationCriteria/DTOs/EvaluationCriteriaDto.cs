namespace AWM.Service.Application.Features.Defense.EvaluationCriteria.DTOs;

public record EvaluationCriteriaDto(
    int Id,
    int WorkTypeId,
    string CriteriaName,
    int MaxScore,
    decimal Weight,
    int? OrgUnitId = null,
    int? SpecialityId = null,
    int? DefenseStageType = null,
    int SortOrder = 0
);

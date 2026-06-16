namespace AWM.Service.Application.Features.Workflow.Checks.DTOs;

public record CheckConfigurationDto(
    int Id,
    int OrgUnitId,
    int CheckTypeId,
    string CheckTypeName,
    string? CheckTypeCode,
    int? SpecialityId,
    string? SpecialityTitle,
    decimal? MinimumPassValue,
    bool IsActive);

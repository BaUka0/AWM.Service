using System;

namespace AWM.Service.WebAPI.Common.Contracts.Responses.Directions;

public record DirectionSummaryResponse(
    long Id,
    int OrgUnitId,
    int SemesterId,
    string TitleRu,
    string? TitleKz,
    string? TitleEn,
    int CurrentStateId,
    DateTime CreatedAt,
    int CreatedBy,
    string CreatorFullName,
    string CreatorPositionTitle);

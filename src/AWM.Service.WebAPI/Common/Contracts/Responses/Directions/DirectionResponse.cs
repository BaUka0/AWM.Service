using System;

namespace AWM.Service.WebAPI.Common.Contracts.Responses.Directions;

public record DirectionResponse(
    long Id,
    int OrgUnitId,
    int SemesterId,
    int WorkTypeId,
    string TitleRu,
    string? TitleKz,
    string? TitleEn,
    string? DescriptionRu,
    string? DescriptionKz,
    string? DescriptionEn,
    int CurrentStateId,
    string CurrentStateName,
    string CurrentStateDisplayName,
    DateTime? SubmittedAt,
    DateTime? ReviewedAt,
    int? ReviewedBy,
    string? ReviewComment,
    DateTime CreatedAt,
    int CreatedBy);

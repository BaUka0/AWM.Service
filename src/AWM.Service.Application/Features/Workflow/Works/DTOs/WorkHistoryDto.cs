using System;

namespace AWM.Service.Application.Features.Workflow.Works.DTOs;

public record WorkHistoryDto(
    long Id,
    int? FromStateId,
    string? FromStateName,
    int ToStateId,
    string ToStateName,
    DateTime TransitionDate,
    string? Comment);

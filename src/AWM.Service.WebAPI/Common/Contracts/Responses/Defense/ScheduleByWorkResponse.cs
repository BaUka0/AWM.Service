using System;
using System.Collections.Generic;

namespace AWM.Service.WebAPI.Common.Contracts.Responses.Defense;

public record CommissionMemberResponse(
    string Role,
    string Name);

public record ScheduleByWorkResponse(
    long? ScheduleId,
    string? DefenseDate,
    string? DefenseTime,
    string? Location,
    int? CommissionId,
    string? CommissionName,
    IReadOnlyList<CommissionMemberResponse>? Members,
    bool IsReconciliationStarted,
    decimal? AverageScore);

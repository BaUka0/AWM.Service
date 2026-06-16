using System;

namespace AWM.Service.WebAPI.Common.Contracts.Requests.Defense;

public sealed record UpdateScheduleRequest(
    int? CommissionId,
    DateTime? DefenseDate,
    string? Location);

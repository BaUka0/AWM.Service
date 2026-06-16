using System;
using System.Collections.Generic;

namespace AWM.Service.WebAPI.Common.Contracts.Responses.Works;

public record WorkHistoryResponse(
    long Id,
    int? FromStateId,
    string? FromStateName,
    int ToStateId,
    string ToStateName,
    DateTime TransitionDate,
    string? Comment);

namespace AWM.Service.Application.Features.Common.Periods.Commands.ApproveInitialPeriods;

using System;
using System.Collections.Generic;
using AWM.Service.Domain.CommonDomain.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

public record PeriodSettingsDto(WorkflowStage WorkflowStage, DateTime StartDate, DateTime EndDate);

public record ApproveInitialPeriodsCommand(
    int DepartmentId,
    int AcademicYearId,
    IReadOnlyList<PeriodSettingsDto> Periods) : IRequest<Result>;

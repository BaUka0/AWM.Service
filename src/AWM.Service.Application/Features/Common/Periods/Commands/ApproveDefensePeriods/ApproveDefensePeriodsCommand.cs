namespace AWM.Service.Application.Features.Common.Periods.Commands.ApproveDefensePeriods;

using System.Collections.Generic;
using AWM.Service.Application.Features.Common.Periods.Commands.ApproveInitialPeriods;
using KDS.Primitives.FluentResult;
using MediatR;

public record ApproveDefensePeriodsCommand(
    int DepartmentId,
    int AcademicYearId,
    IReadOnlyList<PeriodSettingsDto> Periods) : IRequest<Result>;

namespace AWM.Service.Application.Features.Common.Periods.Commands.UpdatePeriod;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record UpdatePeriodCommand : IRequest<Result>
{
    public int PeriodId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool? IsActive { get; init; }
}

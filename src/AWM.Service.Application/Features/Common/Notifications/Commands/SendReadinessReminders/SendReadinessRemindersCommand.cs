namespace AWM.Service.Application.Features.Common.Notifications.Commands.SendReadinessReminders;

using KDS.Primitives.FluentResult;
using MediatR;

public sealed record SendReadinessRemindersCommand : IRequest<Result<int>>
{
    public int DepartmentId { get; init; }
    public int AcademicYearId { get; init; }
}

namespace AWM.Service.Application.Features.Edu.Staff.Commands.UpdateStaffWorkload;

using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Command to update the workload (MaxStudentsLoad) of a staff member.
/// </summary>
public sealed record UpdateStaffWorkloadCommand(
    int StaffId,
    int MaxStudentsLoad) : IRequest<Result>;

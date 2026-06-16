using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.AutoDistributeStudents;

public sealed record AutoDistributeStudentsCommand(
    int OrgUnitId,
    int SemesterId,
    int CommissionTypeId,
    int? PreDefenseNumber = null,
    int? SpecialityId = null) : IRequest<Result>;

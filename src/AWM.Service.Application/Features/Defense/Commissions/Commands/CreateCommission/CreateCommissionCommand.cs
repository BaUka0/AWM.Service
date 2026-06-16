using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.CreateCommission;

public sealed record CreateCommissionCommand(
    int OrgUnitId,
    int SemesterId,
    int? SpecialityId,
    int CommissionTypeId,
    int? PreDefenseNumber,
    string? Name,
    int ChairmanUserId,
    int SecretaryUserId,
    IReadOnlyList<int> MemberUserIds) : IRequest<Result<int>>;

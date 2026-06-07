using AWM.Service.Application.Features.Defense.Commissions.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Queries.GetCommissionById;

public sealed class GetCommissionByIdQueryHandler : IRequestHandler<GetCommissionByIdQuery, Result<CommissionDto>>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUserRepository _userRepository;

    public GetCommissionByIdQueryHandler(ICommissionRepository commissionRepository, IUserRepository userRepository)
    {
        _commissionRepository = commissionRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<CommissionDto>> Handle(GetCommissionByIdQuery request, CancellationToken cancellationToken)
    {
        var commission = await _commissionRepository.GetByIdWithAssignmentsAsync(request.Id, cancellationToken);
        if (commission == null)
            return Result.Failure<CommissionDto>(new Error("Commission.NotFound", $"Commission with ID {request.Id} not found."));

        var userIds = commission.Assignments
            .Where(a => a.IsActive && !a.IsDeleted)
            .Select(a => a.UserId)
            .Distinct()
            .ToList();

        var users = userIds.Count > 0
            ? await _userRepository.GetByIdsAsync(userIds, cancellationToken)
            : Array.Empty<AWM.Service.Domain.University.User>();

        var userMap = users.ToDictionary(u => u.Id);

        var members = commission.Assignments
            .Where(a => a.IsActive && !a.IsDeleted)
            .Select(a =>
            {
                userMap.TryGetValue(a.UserId, out var user);
                var fullName = user != null
                    ? $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim()
                    : "Unknown";
                return new CommissionMemberDto(a.UserId, fullName, (int)a.RoleType);
            })
            .ToList();

        var dto = new CommissionDto(
            commission.Id,
            commission.Name ?? string.Empty,
            commission.CommissionTypeId,
            commission.PreDefenseNumber,
            commission.OrgUnitId,
            commission.SpecialityId,
            commission.SemesterId,
            members);

        return Result.Success(dto);
    }
}

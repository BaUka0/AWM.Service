using AWM.Service.Application.Features.Defense.Commissions.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.Commissions.Queries.GetCommissions;

public sealed class GetCommissionsQueryHandler : IRequestHandler<GetCommissionsQuery, Result<IReadOnlyList<CommissionDto>>>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUserRepository _userRepository;

    public GetCommissionsQueryHandler(ICommissionRepository commissionRepository, IUserRepository userRepository)
    {
        _commissionRepository = commissionRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<IReadOnlyList<CommissionDto>>> Handle(GetCommissionsQuery request, CancellationToken cancellationToken)
    {
        var commissions = await _commissionRepository.GetByOrgUnitAsync(request.OrgUnitId, request.SemesterId, cancellationToken);

        if (request.SpecialityId.HasValue)
            commissions = commissions.Where(c => c.SpecialityId == null || c.SpecialityId == request.SpecialityId.Value).ToList();

        var allUserIds = commissions
            .SelectMany(c => c.Assignments.Select(a => a.UserId))
            .Distinct()
            .ToList();

        var users = allUserIds.Count > 0
            ? await _userRepository.GetByIdsAsync(allUserIds, cancellationToken)
            : Array.Empty<AWM.Service.Domain.University.User>();

        var userMap = users.ToDictionary(u => u.Id);

        var result = commissions.Select(commission =>
        {
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

            return new CommissionDto(
                commission.Id,
                commission.Name ?? string.Empty,
                commission.CommissionTypeId,
                commission.PreDefenseNumber,
                commission.OrgUnitId,
                commission.SpecialityId,
                commission.SemesterId,
                members);
        }).ToList();

        return Result.Success<IReadOnlyList<CommissionDto>>(result);
    }
}

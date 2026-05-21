namespace AWM.Service.Application.Features.Defense.Commissions.Queries.GetCommissionsByDepartment;

using AWM.Service.Application.Features.Defense.Commissions.DTOs;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving all commissions for a department in a given academic year.
/// </summary>
public sealed class GetCommissionsByDepartmentQueryHandler
    : IRequestHandler<GetCommissionsByDepartmentQuery, Result<IReadOnlyList<CommissionDto>>>
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly IUserRepository _userRepository;

    public GetCommissionsByDepartmentQueryHandler(
        ICommissionRepository commissionRepository,
        IUserRepository userRepository)
    {
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Result<IReadOnlyList<CommissionDto>>> Handle(
        GetCommissionsByDepartmentQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var commissions = await _commissionRepository.GetByDepartmentAsync(
                request.OrgUnitId,
                request.SemesterId,
                cancellationToken);

            var allUserIds = commissions
                .SelectMany(c => c.Assignments)
                .Select(a => a.UserId)
                .Distinct()
                .ToList();

            var users = await _userRepository.GetByIdsAsync(allUserIds, cancellationToken);
            var usersDict = users.ToDictionary(u => u.Id, u => u.Email ?? u.FirstName);

            var dtos = commissions
                .Select(c =>
                {
                    var chairman = c.Assignments.FirstOrDefault(a => a.RoleType == StaffRoleType.CommissionChairman && a.IsActive);
                    var secretary = c.Assignments.FirstOrDefault(a => a.RoleType == StaffRoleType.CommissionSecretary && a.IsActive);

                    return new CommissionDto
                    {
                        Id = c.Id,
                        OrgUnitId = c.OrgUnitId,
                        SemesterId = c.SemesterId,
                        CommissionType = c.CommissionTypeId.ToString(),
                        Name = c.Name,
                        PreDefenseNumber = c.PreDefenseNumber,
                        MemberCount = c.Assignments.Count(a => a.IsActive),
                        ChairmanName = chairman != null && usersDict.TryGetValue(chairman.UserId, out var cName) ? cName : null,
                        SecretaryName = secretary != null && usersDict.TryGetValue(secretary.UserId, out var sName) ? sName : null,
                        CreatedAt = c.CreatedAt
                    };
                })
                .ToList();

            return Result.Success<IReadOnlyList<CommissionDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<CommissionDto>>(
                new Error("InternalError", $"An error occurred while retrieving commissions: {ex.Message}"));
        }
    }
}

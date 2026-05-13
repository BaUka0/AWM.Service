namespace AWM.Service.Application.Features.Defense.Commissions.Queries.GetCommissionsByDepartment;

using AWM.Service.Application.Features.Defense.Commissions.DTOs;
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
                request.DepartmentId,
                request.AcademicYearId,
                cancellationToken);

            var allUserIds = commissions
                .SelectMany(c => c.Members)
                .Select(m => m.UserId)
                .Distinct()
                .ToList();

            var users = await _userRepository.GetByIdsAsync(allUserIds, cancellationToken);
            var usersDict = users.ToDictionary(u => u.Id, u => u.Login);

            var dtos = commissions
                .Select(c =>
                {
                    var chairman = c.Members.FirstOrDefault(m => m.RoleInCommission == AWM.Service.Domain.Defense.Enums.RoleInCommission.Chairman);
                    var secretary = c.Members.FirstOrDefault(m => m.RoleInCommission == AWM.Service.Domain.Defense.Enums.RoleInCommission.Secretary);

                    return new CommissionDto
                    {
                        Id = c.Id,
                        DepartmentId = c.DepartmentId,
                        AcademicYearId = c.AcademicYearId,
                        CommissionType = c.CommissionType.ToString(),
                        Name = c.Name,
                        PreDefenseNumber = c.PreDefenseNumber,
                        MemberCount = c.Members.Count,
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

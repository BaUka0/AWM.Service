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

    public GetCommissionsByDepartmentQueryHandler(ICommissionRepository commissionRepository)
    {
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
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

            var dtos = commissions
                .Select(c => new CommissionDto
                {
                    Id = c.Id,
                    DepartmentId = c.DepartmentId,
                    AcademicYearId = c.AcademicYearId,
                    CommissionType = c.CommissionType.ToString(),
                    Name = c.Name,
                    PreDefenseNumber = c.PreDefenseNumber,
                    MemberCount = c.Members.Count,
                    CreatedAt = c.CreatedAt
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

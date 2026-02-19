namespace AWM.Service.Application.Features.Defense.Commissions.Queries.GetCommissionById;

using AWM.Service.Application.Features.Defense.Commissions.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving a specific commission by ID with its members.
/// </summary>
public sealed class GetCommissionByIdQueryHandler
    : IRequestHandler<GetCommissionByIdQuery, Result<CommissionDetailDto>>
{
    private readonly ICommissionRepository _commissionRepository;

    public GetCommissionByIdQueryHandler(ICommissionRepository commissionRepository)
    {
        _commissionRepository = commissionRepository ?? throw new ArgumentNullException(nameof(commissionRepository));
    }

    public async Task<Result<CommissionDetailDto>> Handle(
        GetCommissionByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var commission = await _commissionRepository.GetByIdWithMembersAsync(
                request.CommissionId, cancellationToken);

            if (commission is null)
            {
                return Result.Failure<CommissionDetailDto>(
                    new Error("NotFound.Commission",
                        $"Commission with ID {request.CommissionId} not found."));
            }

            var dto = new CommissionDetailDto
            {
                Id = commission.Id,
                DepartmentId = commission.DepartmentId,
                AcademicYearId = commission.AcademicYearId,
                CommissionType = commission.CommissionType.ToString(),
                Name = commission.Name,
                PreDefenseNumber = commission.PreDefenseNumber,
                CreatedAt = commission.CreatedAt,
                CreatedBy = commission.CreatedBy,
                LastModifiedAt = commission.LastModifiedAt,
                LastModifiedBy = commission.LastModifiedBy,
                Members = commission.Members
                    .Select(m => new CommissionMemberDto
                    {
                        Id = m.Id,
                        CommissionId = m.CommissionId,
                        UserId = m.UserId,
                        RoleInCommission = m.RoleInCommission.ToString(),
                        CreatedAt = m.CreatedAt
                    })
                    .ToList()
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<CommissionDetailDto>(
                new Error("InternalError", $"An error occurred while retrieving the commission: {ex.Message}"));
        }
    }
}

namespace AWM.Service.Application.Features.Thesis.Directions.Queries.GetDirectionById;

using AWM.Service.Application.Features.Thesis.Directions.DTOs;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving a direction by ID with full details.
/// </summary>
public sealed class GetDirectionByIdQueryHandler 
    : IRequestHandler<GetDirectionByIdQuery, Result<DirectionDetailDto>>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;

    public GetDirectionByIdQueryHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        IStaffAssignmentRepository staffAssignmentRepository)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
    }

    public async Task<Result<DirectionDetailDto>> Handle(
        GetDirectionByIdQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // Get direction from repository
            var direction = await _directionRepository
                .GetByIdAsync(request.Id, cancellationToken);

            if (direction is null)
            {
                return Result.Failure<DirectionDetailDto>(new Error(
                    "Direction.NotFound", 
                    $"Direction with ID {request.Id} not found."));
            }

            // Check if deleted (unless explicitly included)
            if (direction.IsDeleted && !request.IncludeDeleted)
            {
                return Result.Failure<DirectionDetailDto>(new Error(
                    "Direction.Deleted", 
                    $"Direction with ID {request.Id} has been deleted."));
            }

            // Get supervisor from StaffAssignments
            var supervisorAssignment = (await _staffAssignmentRepository.GetByTargetAsync(
                "Direction", direction.Id, cancellationToken))
                .FirstOrDefault(a => a.RoleType == (int)StaffRoleType.Supervisor && a.IsActive && !a.IsDeleted);

            // Map to detailed DTO
            var currentState = await _workflowRepository.GetStateByIdAsync(direction.CurrentStateId, cancellationToken);
            var result = new DirectionDetailDto
            {
                Id = direction.Id,
                OrgUnitId = direction.OrgUnitId,
                SupervisorUserId = supervisorAssignment?.UserId ?? 0,
                SemesterId = direction.SemesterId,
                WorkTypeId = direction.WorkTypeId,
                TitleRu = direction.TitleRu,
                TitleKz = direction.TitleKz,
                TitleEn = direction.TitleEn,
                DescriptionRu = direction.DescriptionRu,
                DescriptionKz = direction.DescriptionKz,
                DescriptionEn = direction.DescriptionEn,
                CurrentStateId = direction.CurrentStateId,
                CurrentStateName = currentState?.SystemName,
                CurrentStateDisplayName = currentState?.DisplayName,
                SubmittedAt = direction.SubmittedAt,
                ReviewedAt = direction.ReviewedAt,
                ReviewedBy = direction.ReviewedBy,
                ReviewComment = direction.ReviewComment,
                CreatedAt = direction.CreatedAt,
                CreatedBy = direction.CreatedBy,
                LastModifiedAt = direction.LastModifiedAt,
                LastModifiedBy = direction.LastModifiedBy,
                IsDeleted = direction.IsDeleted,
                DeletedAt = direction.DeletedAt,
                DeletedBy = direction.DeletedBy,
                TopicsCount = direction.Topics?.Count ?? 0
            };

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<DirectionDetailDto>(
                new Error("InternalError", ex.Message));
        }
    }
}

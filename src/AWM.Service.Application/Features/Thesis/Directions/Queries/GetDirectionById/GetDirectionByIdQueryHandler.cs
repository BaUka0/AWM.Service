namespace AWM.Service.Application.Features.Thesis.Directions.Queries.GetDirectionById;

using AWM.Service.Application.Features.Thesis.Directions.DTOs;
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

    public GetDirectionByIdQueryHandler(
        IDirectionRepository directionRepository)
    {
        _directionRepository = directionRepository;
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

            // Map to detailed DTO
            var result = new DirectionDetailDto
            {
                Id = direction.Id,
                DepartmentId = direction.DepartmentId,
                SupervisorId = direction.SupervisorId,
                AcademicYearId = direction.AcademicYearId,
                WorkTypeId = direction.WorkTypeId,
                TitleRu = direction.TitleRu,
                TitleKz = direction.TitleKz,
                TitleEn = direction.TitleEn,
                Description = direction.Description,
                CurrentStateId = direction.CurrentStateId,
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
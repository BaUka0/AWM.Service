namespace AWM.Service.Application.Features.Thesis.Directions.Commands.CreateDirection;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for creating a new research direction.
/// </summary>
public sealed class CreateDirectionCommandHandler
    : IRequestHandler<CreateDirectionCommand, Result<long>>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IOrganizationLookupRepository _organizationLookupRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        IOrganizationLookupRepository organizationLookupRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _organizationLookupRepository = organizationLookupRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<long>> Handle(
        CreateDirectionCommand request,
        CancellationToken cancellationToken)
    {
        // Validate department exists
        var department = await _organizationLookupRepository
            .GetDepartmentByIdAsync(request.DepartmentId, cancellationToken);

        if (department is null)
        {
            return Result.Failure<long>(new Error(
                "404",
                $"Department with ID {request.DepartmentId} not found."));
        }

        // Validate work type exists
        var workType = await _workflowRepository
            .GetWorkTypeByIdAsync(request.WorkTypeId, cancellationToken);

        if (workType is null)
        {
            return Result.Failure<long>(new Error(
                "404",
                $"Work type with ID {request.WorkTypeId} not found."));
        }

        // Get initial "Draft" state for this work type
        var draftState = await _workflowRepository
            .GetStateBySystemNameAsync(request.WorkTypeId, "Draft", cancellationToken);

        if (draftState is null)
        {
            return Result.Failure<long>(new Error(
                "404",
                $"Draft state not found for work type {request.WorkTypeId}."));
        }

        var userId = _currentUserProvider.UserId;
        if (!userId.HasValue)
        {
            return Result.Failure<long>(new Error("401", "User ID is not available."));
        }

        try
        {
            // Create direction entity using domain constructor
            var direction = new Direction(
                departmentId: request.DepartmentId,
                supervisorId: userId.Value, // Use current user ID as supervisor ID
                academicYearId: request.AcademicYearId,
                workTypeId: request.WorkTypeId,
                titleRu: request.TitleRu,
                draftStateId: draftState.Id,
                titleKz: request.TitleKz,
                titleEn: request.TitleEn,
                description: request.Description);

            // Save to repository
            await _directionRepository.AddAsync(direction, cancellationToken);

            return Result.Success(direction.Id);
        }
        catch (ArgumentException ex)
        {
            // Domain validation errors (from entity constructor)
            return Result.Failure<long>(new Error("400", ex.Message));
        }
        catch (Exception ex)
        {
            // Unexpected errors
            return Result.Failure<long>(new Error("500", ex.Message));
        }
    }
}
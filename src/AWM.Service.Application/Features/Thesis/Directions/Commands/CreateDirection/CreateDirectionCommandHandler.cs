namespace AWM.Service.Application.Features.Thesis.Directions.Commands.CreateDirection;

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

    public CreateDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        IOrganizationLookupRepository organizationLookupRepository)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _organizationLookupRepository = organizationLookupRepository;
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
                "Department.NotFound", 
                $"Department with ID {request.DepartmentId} not found."));
        }

        // Validate work type exists
        var workType = await _workflowRepository
            .GetWorkTypeByIdAsync(request.WorkTypeId, cancellationToken);
        
        if (workType is null)
        {
            return Result.Failure<long>(new Error(
                "WorkType.NotFound", 
                $"Work type with ID {request.WorkTypeId} not found."));
        }

        // Get initial "Draft" state for this work type
        var draftState = await _workflowRepository
            .GetStateBySystemNameAsync(request.WorkTypeId, "Draft", cancellationToken);
        
        if (draftState is null)
        {
            return Result.Failure<long>(new Error(
                "State.NotFound", 
                $"Draft state not found for work type {request.WorkTypeId}."));
        }

        try
        {
            // Create direction entity using domain constructor
            var direction = new Direction(
                departmentId: request.DepartmentId,
                supervisorId: request.SupervisorId,
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
            return Result.Failure<long>(new Error("Validation.Error", ex.Message));
        }
        catch (Exception ex)
        {
            // Unexpected errors
            return Result.Failure<long>(new Error("InternalError", ex.Message));
        }
    }
}
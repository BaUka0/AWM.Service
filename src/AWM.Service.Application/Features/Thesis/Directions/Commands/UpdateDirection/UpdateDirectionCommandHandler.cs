namespace AWM.Service.Application.Features.Thesis.Directions.Commands.UpdateDirection;

using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for updating a research direction.
/// </summary>
public sealed class UpdateDirectionCommandHandler 
    : IRequestHandler<UpdateDirectionCommand, Result>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;

    public UpdateDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
    }

    public async Task<Result> Handle(
        UpdateDirectionCommand request, 
        CancellationToken cancellationToken)
    {
        // Get existing direction
        var direction = await _directionRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (direction is null)
        {
            return Result.Failure(new Error(
                "Direction.NotFound", 
                $"Direction with ID {request.Id} not found."));
        }

        // Check if soft-deleted
        if (direction.IsDeleted)
        {
            return Result.Failure(new Error(
                "Direction.Deleted", 
                $"Direction with ID {request.Id} has been deleted."));
        }

        // Verify direction is in draft state (only draft directions can be edited)
        var draftState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Draft", cancellationToken);

        if (draftState is null)
        {
            return Result.Failure(new Error(
                "State.NotFound", 
                "Draft state not found for this work type."));
        }

        if (direction.CurrentStateId != draftState.Id)
        {
            return Result.Failure(new Error(
                "Direction.NotEditable", 
                "Only draft directions can be edited. Current state does not allow modifications."));
        }

        try
        {
            // Update using domain method
            direction.UpdateContent(
                titleRu: request.TitleRu,
                titleKz: request.TitleKz,
                titleEn: request.TitleEn,
                description: request.Description);

            // Update audit fields manually (since domain method doesn't handle LastModifiedBy)
            // Note: If Direction has SetLastModified method, use it instead
            var lastModifiedAtProperty = typeof(Domain.Thesis.Entities.Direction)
                .GetProperty("LastModifiedAt", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var lastModifiedByProperty = typeof(Domain.Thesis.Entities.Direction)
                .GetProperty("LastModifiedBy", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            lastModifiedAtProperty?.SetValue(direction, DateTime.UtcNow);
            lastModifiedByProperty?.SetValue(direction, request.ModifiedBy);

            // Save changes
            await _directionRepository.UpdateAsync(direction, cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            // Domain validation errors (from entity method)
            return Result.Failure(new Error("Validation.Error", ex.Message));
        }
        catch (Exception ex)
        {
            // Unexpected errors
            return Result.Failure(new Error("InternalError", ex.Message));
        }
    }
}
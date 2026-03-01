namespace AWM.Service.Application.Features.Thesis.Directions.Commands.UpdateDirection;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for updating a research direction.
/// </summary>
public sealed class UpdateDirectionCommandHandler
    : IRequestHandler<UpdateDirectionCommand, Result>
{
    private readonly IDirectionRepository _directionRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateDirectionCommandHandler> _logger;

    public UpdateDirectionCommandHandler(
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<UpdateDirectionCommandHandler> logger)
    {
        _directionRepository = directionRepository;
        _workflowRepository = workflowRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        UpdateDirectionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Attempting to update direction ID={DirectionId} by User={UserId}", request.Id, userId);

        // Get existing direction (tracked)
        var direction = await _directionRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (direction is null)
        {
            _logger.LogWarning("UpdateDirection failed: Direction ID={DirectionId} not found.", request.Id);
            return Result.Failure(new Error(
                "404",
                $"Direction with ID {request.Id} not found."));
        }

        // Check if soft-deleted
        if (direction.IsDeleted)
        {
            _logger.LogWarning("UpdateDirection failed: Direction ID={DirectionId} is deleted.", request.Id);
            return Result.Failure(new Error(
                "409",
                $"Direction with ID {request.Id} has been deleted."));
        }

        // Verify direction is in draft state (only draft directions can be edited)
        var draftState = await _workflowRepository
            .GetStateBySystemNameAsync(direction.WorkTypeId, "Draft", cancellationToken);

        if (draftState is null)
        {
            _logger.LogError("UpdateDirection failed: Draft state not found for WorkType ID={WorkTypeId}", direction.WorkTypeId);
            return Result.Failure(new Error(
                "404",
                "Draft state not found for this work type."));
        }

        if (direction.CurrentStateId != draftState.Id)
        {
            _logger.LogWarning("UpdateDirection failed: Direction ID={DirectionId} is in state {StateId}, not Draft.",
                request.Id, direction.CurrentStateId);
            return Result.Failure(new Error(
                "409",
                "Only draft directions can be edited. Current state does not allow modifications."));
        }

        if (!userId.HasValue)
        {
            _logger.LogWarning("UpdateDirection failed: User ID is not available.");
            return Result.Failure(new Error("401", "User ID is not available."));
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
            lastModifiedByProperty?.SetValue(direction, userId.Value);

            // Save changes
            await _directionRepository.UpdateAsync(direction, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully updated direction ID={DirectionId}", request.Id);
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "UpdateDirection validation failed for ID={DirectionId}: {Message}", request.Id, ex.Message);
            return Result.Failure(new Error("400", ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateDirection failed for ID={DirectionId}", request.Id);
            return Result.Failure(new Error("500", ex.Message));
        }
    }
}
namespace AWM.Service.Application.Features.Workflow.Commands.UpdateWorkType;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for UpdateWorkTypeCommand.
/// </summary>
public sealed class UpdateWorkTypeCommandHandler : IRequestHandler<UpdateWorkTypeCommand, Result>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateWorkTypeCommandHandler(
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(UpdateWorkTypeCommand request, CancellationToken cancellationToken)
    {
        var workType = await _workflowRepository.GetWorkTypeByIdAsync(request.Id, cancellationToken);

        if (workType is null)
        {
            return Result.Failure(new Error("NotFound.WorkType", $"Work type with ID {request.Id} not found."));
        }

        if (workType.IsDeleted)
        {
            return Result.Failure(new Error("Conflict.WorkType", "Cannot update a deleted work type."));
        }

        try
        {
            workType.Update(request.Name, request.SpecialityLevelId, _currentUserProvider.UserId ?? 0);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(new Error("Validation.WorkType", ex.Message));
        }

        await _workflowRepository.UpdateWorkTypeAsync(workType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

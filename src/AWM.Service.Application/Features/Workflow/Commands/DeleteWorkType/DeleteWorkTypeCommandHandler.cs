namespace AWM.Service.Application.Features.Workflow.Commands.DeleteWorkType;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for DeleteWorkTypeCommand.
/// </summary>
public sealed class DeleteWorkTypeCommandHandler : IRequestHandler<DeleteWorkTypeCommand, Result>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public DeleteWorkTypeCommandHandler(
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(DeleteWorkTypeCommand request, CancellationToken cancellationToken)
    {
        var workType = await _workflowRepository.GetWorkTypeByIdAsync(request.Id, cancellationToken);

        if (workType is null)
        {
            return Result.Failure(new Error("NotFound.WorkType", $"Work type with ID {request.Id} not found."));
        }

        if (workType.IsDeleted)
        {
            return Result.Failure(new Error("Conflict.WorkType", "Work type is already deleted."));
        }

        workType.Delete(_currentUserProvider.UserId ?? 0);
        await _workflowRepository.UpdateWorkTypeAsync(workType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

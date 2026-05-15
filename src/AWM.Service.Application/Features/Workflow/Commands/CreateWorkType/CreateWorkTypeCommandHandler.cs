namespace AWM.Service.Application.Features.Workflow.Commands.CreateWorkType;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf.Entities;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for CreateWorkTypeCommand.
/// </summary>
public sealed class CreateWorkTypeCommandHandler : IRequestHandler<CreateWorkTypeCommand, Result<int>>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateWorkTypeCommandHandler(
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _workflowRepository = workflowRepository ?? throw new ArgumentNullException(nameof(workflowRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<int>> Handle(CreateWorkTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var workType = new WorkType(request.Name, _currentUserProvider.UserId ?? 0, request.DegreeLevelId);
            await _workflowRepository.AddWorkTypeAsync(workType, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(workType.Id);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<int>(new Error("Validation.WorkType", ex.Message));
        }
    }
}

namespace AWM.Service.Application.Features.Common.Dictionaries.Queries.GetWorkflowStages;

using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Repositories;
using MediatR;

/// <summary>
/// Query to get all workflow stages (reference dictionary).
/// </summary>
public sealed record GetWorkflowStagesQuery : IRequest<IReadOnlyList<WorkflowStage>>;

/// <summary>
/// Handler for GetWorkflowStagesQuery.
/// </summary>
public sealed class GetWorkflowStagesQueryHandler : IRequestHandler<GetWorkflowStagesQuery, IReadOnlyList<WorkflowStage>>
{
    private readonly IWorkflowStageRepository _repository;

    public GetWorkflowStagesQueryHandler(IWorkflowStageRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IReadOnlyList<WorkflowStage>> Handle(GetWorkflowStagesQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}

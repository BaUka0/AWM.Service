namespace AWM.Service.Application.Features.Workflow.WorkTypes.Queries.GetWorkTypes;
using KDS.Primitives.FluentResult;
using MediatR;
using AWM.Service.Domain.Repositories;
using AWM.Service.Application.Features.Workflow.WorkTypes.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public record GetWorkTypesQuery() : IRequest<Result<IReadOnlyList<WorkTypeDto>>>;

public class GetWorkTypesQueryHandler : IRequestHandler<GetWorkTypesQuery, Result<IReadOnlyList<WorkTypeDto>>>
{
    private readonly IWorkflowRepository _repo;
    public GetWorkTypesQueryHandler(IWorkflowRepository repo) { _repo = repo; }
    public async Task<Result<IReadOnlyList<WorkTypeDto>>> Handle(GetWorkTypesQuery request, CancellationToken ct)
    {
        var items = await _repo.GetAllWorkTypesAsync(ct);
        var dtos = items.Where(x => !x.IsDeleted).Select(x => new WorkTypeDto(x.Id, x.Name, x.SpecialityLevelId)).ToList();
        return Result.Success<IReadOnlyList<WorkTypeDto>>(dtos);
    }
}

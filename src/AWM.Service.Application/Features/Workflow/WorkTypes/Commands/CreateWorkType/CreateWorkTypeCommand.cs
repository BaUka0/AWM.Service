namespace AWM.Service.Application.Features.Workflow.WorkTypes.Commands.CreateWorkType;

using KDS.Primitives.FluentResult;
using MediatR;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Wf.Entities;
using System.Threading;
using System.Threading.Tasks;

public record CreateWorkTypeCommand(string Name, int? SpecialityLevelId) : IRequest<Result<int>>;

public class CreateWorkTypeCommandHandler : IRequestHandler<CreateWorkTypeCommand, Result<int>>
{
    private readonly IWorkflowRepository _repo;
    private readonly IUnitOfWork _uow;
    public CreateWorkTypeCommandHandler(IWorkflowRepository repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result<int>> Handle(CreateWorkTypeCommand request, CancellationToken ct)
    {
        var entity = new WorkType(request.Name, 1, request.SpecialityLevelId);
        await _repo.AddWorkTypeAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success(entity.Id);
    }
}

namespace AWM.Service.Application.Features.Workflow.WorkTypes.Commands.UpdateWorkType;
using KDS.Primitives.FluentResult;
using MediatR;
using AWM.Service.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

public record UpdateWorkTypeCommand(int Id, string Name, int? SpecialityLevelId) : IRequest<Result>;

public class UpdateWorkTypeCommandHandler : IRequestHandler<UpdateWorkTypeCommand, Result>
{
    private readonly IWorkflowRepository _repo;
    private readonly IUnitOfWork _uow;
    public UpdateWorkTypeCommandHandler(IWorkflowRepository repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> Handle(UpdateWorkTypeCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetWorkTypeByIdAsync(request.Id, ct);
        if (entity == null) return Result.Failure(new Error("NotFound", "Not found"));
        entity.Update(request.Name, request.SpecialityLevelId, 1);
        await _repo.UpdateWorkTypeAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

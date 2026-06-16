namespace AWM.Service.Application.Features.Workflow.WorkTypes.Commands.DeleteWorkType;

using KDS.Primitives.FluentResult;
using MediatR;
using AWM.Service.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

public record DeleteWorkTypeCommand(int Id) : IRequest<Result>;

public class DeleteWorkTypeCommandHandler : IRequestHandler<DeleteWorkTypeCommand, Result>
{
    private readonly IWorkflowRepository _repo;
    private readonly IUnitOfWork _uow;
    public DeleteWorkTypeCommandHandler(IWorkflowRepository repo, IUnitOfWork uow) { _repo = repo; _uow = uow; }
    public async Task<Result> Handle(DeleteWorkTypeCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetWorkTypeByIdAsync(request.Id, ct);
        if (entity == null) return Result.Failure(new Error("NotFound", "Not found"));
        entity.Delete(1);
        await _repo.UpdateWorkTypeAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

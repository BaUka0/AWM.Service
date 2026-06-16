using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Checks.Commands.DeleteCheckConfiguration;

public record DeleteCheckConfigurationCommand(int Id) : IRequest<Result<Unit>>;

public sealed class DeleteCheckConfigurationCommandHandler : IRequestHandler<DeleteCheckConfigurationCommand, Result<Unit>>
{
    private readonly ISpecialityCheckTypeRepository _specialityCheckTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCheckConfigurationCommandHandler(
        ISpecialityCheckTypeRepository specialityCheckTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _specialityCheckTypeRepository = specialityCheckTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(DeleteCheckConfigurationCommand request, CancellationToken cancellationToken)
    {
        var config = await _specialityCheckTypeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (config == null)
        {
            return Result.Failure<Unit>(new Error("CheckConfigurations.NotFound", $"Configuration with ID {request.Id} not found."));
        }

        await _specialityCheckTypeRepository.DeleteAsync(config, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}

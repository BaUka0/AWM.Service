using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Checks.Commands.SaveCheckConfiguration;

public record SaveCheckConfigurationCommand(
    int OrgUnitId,
    int CheckTypeId,
    int? SpecialityId,
    decimal? MinimumPassValue,
    bool IsActive) : IRequest<Result<int>>;

public sealed class SaveCheckConfigurationCommandHandler : IRequestHandler<SaveCheckConfigurationCommand, Result<int>>
{
    private readonly ISpecialityCheckTypeRepository _specialityCheckTypeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SaveCheckConfigurationCommandHandler(
        ISpecialityCheckTypeRepository specialityCheckTypeRepository,
        IUnitOfWork unitOfWork)
    {
        _specialityCheckTypeRepository = specialityCheckTypeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(SaveCheckConfigurationCommand request, CancellationToken cancellationToken)
    {
        var existing = await _specialityCheckTypeRepository.GetByCompositeKeyAsync(
            request.OrgUnitId,
            request.CheckTypeId,
            request.SpecialityId,
            cancellationToken);

        if (existing != null)
        {
            existing.Update(request.MinimumPassValue, request.IsActive);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(existing.Id);
        }

        var newConfig = new SpecialityCheckType(
            request.OrgUnitId,
            request.CheckTypeId,
            request.SpecialityId,
            request.MinimumPassValue,
            request.IsActive);

        await _specialityCheckTypeRepository.AddAsync(newConfig, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(newConfig.Id);
    }
}

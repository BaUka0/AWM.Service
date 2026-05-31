using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.EvaluationCriteria.Commands.UpdateCriteria;

public sealed class UpdateCriteriaCommandHandler : IRequestHandler<UpdateCriteriaCommand, Result>
{
    private readonly IEvaluationCriteriaRepository _criteriaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public UpdateCriteriaCommandHandler(
        IEvaluationCriteriaRepository criteriaRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _criteriaRepository = criteriaRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(UpdateCriteriaCommand request, CancellationToken cancellationToken)
    {
        var criteria = await _criteriaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (criteria == null)
        {
            return Result.Failure(new Error("EvaluationCriteria.NotFound", $"Criteria with ID {request.Id} not found."));
        }

        if (!_currentUserProvider.IsAuthenticated || !_currentUserProvider.UserId.HasValue)
            return Result.Failure(new Error("Auth.MissingPrincipal", "Unable to identify the current user."));

        var modifiedBy = _currentUserProvider.UserId.Value;

        try
        {
            criteria.Update(request.CriteriaName, request.MaxScore, request.Weight, modifiedBy, request.DefenseStageType, request.SortOrder);
            await _criteriaRepository.UpdateAsync(criteria, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(new Error(ex.ErrorCode, ex.Message));
        }
    }
}

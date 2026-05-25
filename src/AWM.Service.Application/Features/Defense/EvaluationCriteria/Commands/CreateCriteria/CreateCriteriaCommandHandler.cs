using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

namespace AWM.Service.Application.Features.Defense.EvaluationCriteria.Commands.CreateCriteria;

public sealed class CreateCriteriaCommandHandler : IRequestHandler<CreateCriteriaCommand, Result<int>>
{
    private readonly IEvaluationCriteriaRepository _criteriaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public CreateCriteriaCommandHandler(
        IEvaluationCriteriaRepository criteriaRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _criteriaRepository = criteriaRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<int>> Handle(CreateCriteriaCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.IsAuthenticated || !_currentUserProvider.UserId.HasValue)
            return Result.Failure<int>(new Error("Auth.MissingPrincipal", "Unable to identify the current user."));

        var createdBy = _currentUserProvider.UserId.Value;

        try
        {
            var criteria = new Domain.Defense.Entities.EvaluationCriteria(
                request.WorkTypeId,
                request.CriteriaName,
                request.MaxScore,
                createdBy,
                request.Weight,
                request.OrgUnitId,
                request.SpecialityId);

            await _criteriaRepository.AddAsync(criteria, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(criteria.Id);
        }
        catch (DomainException ex)
        {
            return Result.Failure<int>(new Error(ex.ErrorCode, ex.Message));
        }
    }
}

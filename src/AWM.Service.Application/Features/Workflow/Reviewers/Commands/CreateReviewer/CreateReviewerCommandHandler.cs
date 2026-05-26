using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Reviewers.Commands.CreateReviewer;

public sealed class CreateReviewerCommandHandler : IRequestHandler<CreateReviewerCommand, Result<int>>
{
    private readonly IReviewerRepository _reviewerRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewerCommandHandler(
        IReviewerRepository reviewerRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _reviewerRepository = reviewerRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateReviewerCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<int>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var reviewer = new Reviewer(
            request.FullName,
            _currentUserProvider.UserId.Value,
            request.Position,
            request.AcademicDegree,
            request.Organization,
            request.Email,
            request.Phone);

        await _reviewerRepository.AddAsync(reviewer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(reviewer.Id);
    }
}

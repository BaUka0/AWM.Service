using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Reviewers.Commands.UpdateReviewer;

public sealed class UpdateReviewerCommandHandler : IRequestHandler<UpdateReviewerCommand, Result>
{
    private readonly IReviewerRepository _reviewerRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReviewerCommandHandler(
        IReviewerRepository reviewerRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _reviewerRepository = reviewerRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateReviewerCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var reviewer = await _reviewerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (reviewer == null)
        {
            return Result.Failure(new Error("Reviewers.NotFound", $"Reviewer with ID {request.Id} not found."));
        }

        reviewer.UpdateInfo(
            request.FullName,
            _currentUserProvider.UserId.Value,
            request.Position,
            request.AcademicDegree,
            request.Organization,
            request.Email,
            request.Phone);

        await _reviewerRepository.UpdateAsync(reviewer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

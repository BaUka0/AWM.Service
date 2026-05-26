using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Reviewers.Commands.DeleteReviewer;

public sealed class DeleteReviewerCommandHandler : IRequestHandler<DeleteReviewerCommand, Result>
{
    private readonly IReviewerRepository _reviewerRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReviewerCommandHandler(
        IReviewerRepository reviewerRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _reviewerRepository = reviewerRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteReviewerCommand request, CancellationToken cancellationToken)
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

        reviewer.Delete(_currentUserProvider.UserId.Value);

        await _reviewerRepository.UpdateAsync(reviewer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

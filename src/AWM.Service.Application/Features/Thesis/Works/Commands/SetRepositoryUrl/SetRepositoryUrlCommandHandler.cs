namespace AWM.Service.Application.Features.Thesis.Works.Commands.SetRepositoryUrl;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class SetRepositoryUrlCommandHandler : IRequestHandler<SetRepositoryUrlCommand, Result>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public SetRepositoryUrlCommandHandler(
        IStudentWorkRepository workRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(SetRepositoryUrlCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            var work = await _workRepository.GetByIdAsync(request.WorkId, cancellationToken);
            if (work is null)
                return Result.Failure(new Error("NotFound.Work",
                    $"StudentWork with ID {request.WorkId} not found."));

            work.SetRepositoryUrl(request.RepositoryUrl, userId.Value);

            await _workRepository.UpdateAsync(work, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", ex.Message));
        }
    }
}

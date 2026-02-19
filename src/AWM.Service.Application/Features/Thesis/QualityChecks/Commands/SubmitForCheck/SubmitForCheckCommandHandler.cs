namespace AWM.Service.Application.Features.Thesis.QualityChecks.Commands.SubmitForCheck;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for submitting work for a quality check.
/// Creates an initial (pending/unreviewed) quality check attempt.
/// The expert will later record the actual result via RecordCheckResult.
/// </summary>
public sealed class SubmitForCheckCommandHandler : IRequestHandler<SubmitForCheckCommand, Result<long>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public SubmitForCheckCommandHandler(
        IStudentWorkRepository workRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<long>> Handle(SubmitForCheckCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
            {
                return Result.Failure<long>(new Error("401", "User ID is not available."));
            }

            var work = await _workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
            if (work is null)
            {
                return Result.Failure<long>(new Error("NotFound.Work",
                    $"StudentWork with ID {request.WorkId} not found."));
            }

            // Submit = create a "pending" check record (isPassed: false until expert reviews)
            // The expert will update the result later via RecordCheckResult
            var check = work.AddQualityCheck(
                checkType: request.CheckType,
                isPassed: false,
                expertId: null,
                comment: request.Comment);

            await _workRepository.UpdateAsync(work, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(check.Id);
        }
        catch (InvalidOperationException ioEx)
        {
            return Result.Failure<long>(new Error("BusinessRule.QualityCheck", ioEx.Message));
        }
        catch (Exception ex)
        {
            return Result.Failure<long>(new Error("500", ex.Message));
        }
    }
}

namespace AWM.Service.Application.Features.Thesis.QualityChecks.Commands.SubmitForCheck;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
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
    private readonly IPreDefenseAttemptRepository _attemptRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public SubmitForCheckCommandHandler(
        IStudentWorkRepository workRepository,
        IPreDefenseAttemptRepository attemptRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
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

            // Validate that the student has passed pre-defense before submitting for quality checks
            var attempts = await _attemptRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);
            if (!attempts.Any(a => a.IsPassed))
            {
                return Result.Failure<long>(new Error("BusinessRule.QualityCheck",
                    "Student must pass pre-defense before submitting for quality checks."));
            }

            // Validate check sequence: AntiPlagiarism requires NormControl to be passed
            if (request.CheckType == CheckType.AntiPlagiarism)
            {
                if (!work.HasPassedCheck(CheckType.NormControl))
                {
                    return Result.Failure<long>(new Error("BusinessRule.QualityCheck",
                        "NormControl must be passed before submitting for AntiPlagiarism check."));
                }

                // Rework cycle: if a previous AntiPlagiarism check failed, NormControl must be re-passed
                // (latest NormControl attempt must be newer than latest failed AntiPlagiarism)
                var latestFailedPlagiarism = work.GetLatestCheck(CheckType.AntiPlagiarism);
                var latestNormControl = work.GetLatestCheck(CheckType.NormControl);

                if (latestFailedPlagiarism is not null && !latestFailedPlagiarism.IsPassed
                    && latestNormControl is not null
                    && latestNormControl.AttemptNumber <= latestFailedPlagiarism.AttemptNumber
                    && !latestNormControl.IsPassed)
                {
                    return Result.Failure<long>(new Error("BusinessRule.QualityCheck",
                        "After AntiPlagiarism failure, NormControl must be re-passed before retrying."));
                }
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

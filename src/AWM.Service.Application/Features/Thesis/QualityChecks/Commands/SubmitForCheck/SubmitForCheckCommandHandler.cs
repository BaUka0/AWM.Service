namespace AWM.Service.Application.Features.Thesis.QualityChecks.Commands.SubmitForCheck;

using AWM.Service.Domain.Thesis.Constants;
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
    private readonly IPreDefenseAttemptRepository _attemptRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ICheckTypeRepository _checkTypeRepository;
    private readonly ISpecialityCheckTypeRepository _specialityCheckTypeRepository;
    private readonly IStudentRepository _studentRepository;

    public SubmitForCheckCommandHandler(
        IStudentWorkRepository workRepository,
        IPreDefenseAttemptRepository attemptRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider,
        ICheckTypeRepository checkTypeRepository,
        ISpecialityCheckTypeRepository specialityCheckTypeRepository,
        IStudentRepository studentRepository)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _checkTypeRepository = checkTypeRepository ?? throw new ArgumentNullException(nameof(checkTypeRepository));
        _specialityCheckTypeRepository = specialityCheckTypeRepository ?? throw new ArgumentNullException(nameof(specialityCheckTypeRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
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

            var checkType = await _checkTypeRepository.GetByIdAsync(request.CheckTypeId, cancellationToken);
            if (checkType is null)
            {
                return Result.Failure<long>(new Error("NotFound.CheckType",
                    $"CheckType with ID {request.CheckTypeId} not found."));
            }

            // Validate that the student has passed pre-defense before submitting for quality checks
            var attempts = await _attemptRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);
            if (!attempts.Any(a => a.IsPassed))
            {
                return Result.Failure<long>(new Error("BusinessRule.QualityCheck",
                    "Student must pass pre-defense before submitting for quality checks."));
            }

            // Validate check sequence: AntiPlagiarism requires NormControl and all speciality-specific checks to be passed
            if (checkType.Code == CheckTypeCodes.AntiPlagiarism)
            {
                var normControlCheckType = await _checkTypeRepository.GetByCodeAsync(CheckTypeCodes.NormControl, cancellationToken);
                
                if (normControlCheckType is not null && !work.HasPassedCheck(normControlCheckType.Id))
                {
                    return Result.Failure<long>(new Error("BusinessRule.QualityCheck",
                        "NormControl must be passed before submitting for AntiPlagiarism check."));
                }

                var firstParticipant = work.Participants.FirstOrDefault();
                if (firstParticipant != null)
                {
                    var student = await _studentRepository.GetByIdAsync(firstParticipant.StudentId, cancellationToken);
                    if (student != null)
                    {
                        var mandatoryChecks = await _specialityCheckTypeRepository.GetBySpecialityAsync(student.SpecialityId, cancellationToken);
                        foreach (var mc in mandatoryChecks)
                        {
                            if (!work.HasPassedCheck(mc.CheckTypeId))
                            {
                                return Result.Failure<long>(new Error("BusinessRule.QualityCheck",
                                    $"A mandatory check ({mc.CheckType?.Title}) for your speciality must be passed before submitting for AntiPlagiarism check."));
                            }
                        }
                    }
                }

                // Rework cycle: if a previous AntiPlagiarism check failed, NormControl must be re-passed
                if (normControlCheckType is not null)
                {
                    var latestFailedPlagiarism = work.GetLatestCheck(checkType.Id);
                    var latestNormControl = work.GetLatestCheck(normControlCheckType.Id);

                    if (latestFailedPlagiarism is not null && !latestFailedPlagiarism.IsPassed
                        && latestNormControl is not null
                        && latestNormControl.CreatedAt <= latestFailedPlagiarism.CreatedAt)
                    {
                        return Result.Failure<long>(new Error("BusinessRule.QualityCheck",
                            "After AntiPlagiarism failure, NormControl must be re-passed before retrying."));
                    }
                }
            }

            // Submit = create a "pending" check record (isPassed: false until expert reviews)
            // The expert will update the result later via RecordCheckResult
            var check = work.AddQualityCheck(
                checkTypeId: request.CheckTypeId,
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

namespace AWM.Service.Application.Features.Thesis.QualityChecks.Commands.RecordCheckResult;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for recording a quality check result by an expert.
/// Finds the existing pending QualityCheck (created by SubmitForCheck) by CheckId
/// and updates it in-place with the expert's verdict.
/// </summary>
public sealed class RecordCheckResultCommandHandler : IRequestHandler<RecordCheckResultCommand, Result<long>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public RecordCheckResultCommandHandler(
        IStudentWorkRepository workRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<long>> Handle(RecordCheckResultCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var expertId = _currentUserProvider.UserId;
            if (!expertId.HasValue)
            {
                return Result.Failure<long>(new Error("401", "Expert user ID is not available."));
            }

            var work = await _workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
            if (work is null)
            {
                return Result.Failure<long>(new Error("NotFound.Work",
                    $"StudentWork with ID {request.WorkId} not found."));
            }

            // Find the pending check and record the expert's result in-place
            var check = work.CompleteQualityCheck(
                checkId: request.CheckId,
                expertId: expertId.Value,
                isPassed: request.IsPassed,
                resultValue: request.ResultValue,
                comment: request.Comment,
                documentPath: request.DocumentPath);

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

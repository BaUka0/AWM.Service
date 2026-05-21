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
using System.Linq;
using System.Text.Json;
using AWM.Service.Domain.CommonDomain.Enums;

public sealed class RecordCheckResultCommandHandler : IRequestHandler<RecordCheckResultCommand, Result<long>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IStaffAssignmentRepository _assignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public RecordCheckResultCommandHandler(
        IStudentWorkRepository workRepository,
        IStaffAssignmentRepository assignmentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
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

            // Find the pending check to verify its CheckTypeId
            var check = work.QualityChecks.FirstOrDefault(c => c.Id == request.CheckId);
            if (check is null)
            {
                return Result.Failure<long>(new Error("NotFound.QualityCheck",
                    $"QualityCheck with ID {request.CheckId} not found on this work."));
            }

            // Validate that the user is actually assigned as a QualityExpert for this check type in the work's department
            var assignments = await _assignmentRepository.GetByRoleAsync(
                "Department",
                work.OrgUnitId,
                StaffRoleType.QualityExpert,
                cancellationToken);

            var isAssigned = assignments.Any(a => 
                a.UserId == expertId.Value && 
                ParseCheckTypeIdFromMetadata(a.MetadataJson) == check.CheckTypeId);

            if (!isAssigned)
            {
                return Result.Failure<long>(new Error("BusinessRule.ExpertAuthorization",
                    "You are not assigned as an expert for this quality check type in this department."));
            }

            // Record the expert's result in-place
            var completedCheck = work.CompleteQualityCheck(
                checkId: request.CheckId,
                expertId: expertId.Value,
                isPassed: request.IsPassed,
                resultValue: request.ResultValue,
                comment: request.Comment,
                documentPath: request.DocumentPath);

            await _workRepository.UpdateAsync(work, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(completedCheck.Id);
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

    private static int? ParseCheckTypeIdFromMetadata(string? metadataJson)
    {
        if (string.IsNullOrWhiteSpace(metadataJson)) return null;
        try
        {
            using var doc = JsonDocument.Parse(metadataJson);
            if (doc.RootElement.TryGetProperty("CheckTypeId", out var prop) && prop.ValueKind == JsonValueKind.Number)
            {
                return prop.GetInt32();
            }
        }
        catch
        {
            // Ignore parsing errors
        }
        return null;
    }
}

namespace AWM.Service.Application.Features.Thesis.QualityChecks.Queries.GetPendingChecks;

using AWM.Service.Application.Features.Thesis.QualityChecks.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving all pending quality checks in a department.
/// Returns checks where AssignedExpertId is null (submitted by student, not yet reviewed by an expert).
/// </summary>
public sealed class GetPendingChecksQueryHandler
    : IRequestHandler<GetPendingChecksQuery, Result<IReadOnlyList<QualityCheckDto>>>
{
    private readonly IStudentWorkRepository _workRepository;

    public GetPendingChecksQueryHandler(IStudentWorkRepository workRepository)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
    }

    public async Task<Result<IReadOnlyList<QualityCheckDto>>> Handle(
        GetPendingChecksQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Load all works in the department for the given academic year
            var works = await _workRepository.GetByDepartmentAsync(
                request.DepartmentId,
                request.AcademicYearId,
                cancellationToken);

            // Collect pending quality checks (no expert assigned yet) across all works
            var pendingChecks = works
                .SelectMany(w => w.QualityChecks)
                .Where(c => c.AssignedExpertId is null)
                .Where(c => !request.CheckType.HasValue || c.CheckType == request.CheckType)
                .OrderBy(c => c.CheckedAt)
                .Select(c => new QualityCheckDto
                {
                    Id = c.Id,
                    WorkId = c.WorkId,
                    CheckType = c.CheckType.ToString(),
                    AttemptNumber = c.AttemptNumber,
                    IsPassed = c.IsPassed,
                    ResultValue = c.ResultValue,
                    Comment = c.Comment,
                    DocumentPath = c.DocumentPath,
                    AssignedExpertId = c.AssignedExpertId,
                    CheckedAt = c.CheckedAt
                })
                .ToList();

            return Result.Success<IReadOnlyList<QualityCheckDto>>(pendingChecks);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<QualityCheckDto>>(
                new Error("InternalError", $"An error occurred while retrieving pending checks: {ex.Message}"));
        }
    }
}

namespace AWM.Service.Application.Features.Thesis.QualityChecks.Queries.GetChecksByWork;

using AWM.Service.Application.Features.Thesis.QualityChecks.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving all quality checks for a specific work.
/// </summary>
public sealed class GetChecksByWorkQueryHandler
    : IRequestHandler<GetChecksByWorkQuery, Result<IReadOnlyList<QualityCheckDto>>>
{
    private readonly IStudentWorkRepository _workRepository;

    public GetChecksByWorkQueryHandler(IStudentWorkRepository workRepository)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
    }

    public async Task<Result<IReadOnlyList<QualityCheckDto>>> Handle(
        GetChecksByWorkQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var work = await _workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
            if (work is null)
            {
                return Result.Failure<IReadOnlyList<QualityCheckDto>>(
                    new Error("NotFound.Work", $"StudentWork with ID {request.WorkId} not found."));
            }

            var dtos = work.QualityChecks
                .OrderBy(c => c.CheckType.ToString())
                .ThenBy(c => c.AttemptNumber)
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

            return Result.Success<IReadOnlyList<QualityCheckDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<QualityCheckDto>>(
                new Error("InternalError", $"An error occurred while retrieving quality checks: {ex.Message}"));
        }
    }
}

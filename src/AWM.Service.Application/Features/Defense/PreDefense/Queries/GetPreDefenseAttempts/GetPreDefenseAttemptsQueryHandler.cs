namespace AWM.Service.Application.Features.Defense.PreDefense.Queries.GetPreDefenseAttempts;

using AWM.Service.Application.Features.Defense.PreDefense.DTOs;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for retrieving all pre-defense attempts for a student work.
/// </summary>
public sealed class GetPreDefenseAttemptsQueryHandler
    : IRequestHandler<GetPreDefenseAttemptsQuery, Result<IReadOnlyList<PreDefenseAttemptDto>>>
{
    private readonly IPreDefenseAttemptRepository _attemptRepository;

    public GetPreDefenseAttemptsQueryHandler(IPreDefenseAttemptRepository attemptRepository)
    {
        _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
    }

    public async Task<Result<IReadOnlyList<PreDefenseAttemptDto>>> Handle(
        GetPreDefenseAttemptsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var attempts = await _attemptRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);

            var dtos = attempts
                .OrderBy(a => a.PreDefenseNumber)
                .Select(a => new PreDefenseAttemptDto
                {
                    Id = a.Id,
                    WorkId = a.WorkId,
                    PreDefenseNumber = a.PreDefenseNumber,
                    ScheduleId = a.ScheduleId,
                    AttendanceStatus = a.AttendanceStatus.ToString(),
                    AverageScore = a.AverageScore,
                    IsPassed = a.IsPassed,
                    NeedsRetake = a.NeedsRetake,
                    AttemptDate = a.AttemptDate,
                    CreatedAt = a.CreatedAt
                })
                .ToList();

            return Result.Success<IReadOnlyList<PreDefenseAttemptDto>>(dtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<PreDefenseAttemptDto>>(
                new Error("InternalError", $"An error occurred: {ex.Message}"));
        }
    }
}

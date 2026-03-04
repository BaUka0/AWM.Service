namespace AWM.Service.Application.Features.Defense.PreDefense.Queries.GetFailedPreDefenseStudents;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for querying students who failed pre-defense and may need retake.
/// </summary>
public sealed class GetFailedPreDefenseStudentsQueryHandler
    : IRequestHandler<GetFailedPreDefenseStudentsQuery, Result<IReadOnlyList<FailedPreDefenseStudentDto>>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IPreDefenseAttemptRepository _attemptRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ILogger<GetFailedPreDefenseStudentsQueryHandler> _logger;

    public GetFailedPreDefenseStudentsQueryHandler(
        IStudentWorkRepository workRepository,
        IPreDefenseAttemptRepository attemptRepository,
        ICurrentUserProvider currentUserProvider,
        ILogger<GetFailedPreDefenseStudentsQueryHandler> logger)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<IReadOnlyList<FailedPreDefenseStudentDto>>> Handle(
        GetFailedPreDefenseStudentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<IReadOnlyList<FailedPreDefenseStudentDto>>(
                    new Error("401", "User ID is not available."));

            var works = await _workRepository.GetByDepartmentAsync(
                request.DepartmentId, request.AcademicYearId, cancellationToken);

            var failedStudents = new List<FailedPreDefenseStudentDto>();

            foreach (var work in works)
            {
                var attempts = await _attemptRepository.GetByWorkIdAsync(work.Id, cancellationToken);

                if (!attempts.Any())
                    continue;

                // Filter by pre-defense number if specified
                var relevantAttempts = request.PreDefenseNumber.HasValue
                    ? attempts.Where(a => a.PreDefenseNumber == request.PreDefenseNumber.Value).ToList()
                    : attempts.ToList();

                if (!relevantAttempts.Any())
                    continue;

                // Check if student has passed any pre-defense
                var hasPassed = relevantAttempts.Any(a => a.IsPassed);
                if (hasPassed)
                    continue;

                var latestAttempt = relevantAttempts
                    .OrderByDescending(a => a.PreDefenseNumber)
                    .ThenByDescending(a => a.AttemptDate)
                    .First();

                failedStudents.Add(new FailedPreDefenseStudentDto
                {
                    WorkId = work.Id,
                    LastAttemptNumber = latestAttempt.PreDefenseNumber,
                    LastScore = latestAttempt.AverageScore,
                    CanRetake = latestAttempt.NeedsRetake,
                    LastAttemptDate = latestAttempt.AttemptDate
                });
            }

            return Result.Success<IReadOnlyList<FailedPreDefenseStudentDto>>(failedStudents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetFailedPreDefenseStudents failed for Dept={DeptId}", request.DepartmentId);
            return Result.Failure<IReadOnlyList<FailedPreDefenseStudentDto>>(new Error("500", ex.Message));
        }
    }
}

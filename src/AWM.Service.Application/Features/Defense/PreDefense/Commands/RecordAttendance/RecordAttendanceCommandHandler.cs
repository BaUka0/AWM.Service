namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.RecordAttendance;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Defense.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

/// <summary>
/// Handler for recording a student's attendance at a pre-defense.
/// Updates the PreDefenseAttempt status (Attended / Absent / Excused).
/// </summary>
public sealed class RecordAttendanceCommandHandler : IRequestHandler<RecordAttendanceCommand, Result>
{
    private readonly IPreDefenseAttemptRepository _attemptRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public RecordAttendanceCommandHandler(
        IPreDefenseAttemptRepository attemptRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result> Handle(RecordAttendanceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure(new Error("401", "User ID is not available."));

            var attempt = await _attemptRepository.GetByIdAsync(request.AttemptId, cancellationToken);
            if (attempt is null)
                return Result.Failure(new Error("NotFound.Attempt",
                    $"PreDefenseAttempt with ID {request.AttemptId} not found."));

            // Determine action based on attendance status
            if (request.AttendanceStatus == AttendanceStatus.Attended)
            {
                // No-op: default status is Attended; no state changes needed.
                return Result.Success();
            }

            attempt.MarkAbsent(userId.Value, excused: request.IsExcused);

            await _attemptRepository.UpdateAsync(attempt, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("500", ex.Message));
        }
    }
}

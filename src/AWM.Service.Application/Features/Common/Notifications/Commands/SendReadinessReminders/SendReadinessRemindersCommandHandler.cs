namespace AWM.Service.Application.Features.Common.Notifications.Commands.SendReadinessReminders;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class SendReadinessRemindersCommandHandler
    : IRequestHandler<SendReadinessRemindersCommand, Result<int>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IPreDefenseAttemptRepository _attemptRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly INotificationService _notificationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ILogger<SendReadinessRemindersCommandHandler> _logger;

    public SendReadinessRemindersCommandHandler(
        IStudentWorkRepository workRepository,
        IPreDefenseAttemptRepository attemptRepository,
        IReviewRepository reviewRepository,
        IStudentRepository studentRepository,
        INotificationService notificationService,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider,
        ILogger<SendReadinessRemindersCommandHandler> logger)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<int>> Handle(
        SendReadinessRemindersCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<int>(new Error("401", "User ID is not available."));

            var works = await _workRepository.GetByDepartmentAsync(
                request.DepartmentId, request.AcademicYearId, cancellationToken);

            var notifiedCount = 0;

            foreach (var work in works)
            {
                var attempts = await _attemptRepository.GetByWorkIdAsync(work.Id, cancellationToken);
                var preDefensePassed = attempts.Any(a => a.IsPassed);
                var normPassed = work.HasPassedCheck(CheckType.NormControl);
                var softwarePassed = work.HasPassedCheck(CheckType.SoftwareCheck);
                var plagiarismPassed = work.HasPassedCheck(CheckType.AntiPlagiarism);
                var reviews = await _reviewRepository.GetByWorkIdAsync(work.Id, cancellationToken);
                var hasReview = reviews.Any(r => r.IsUploaded);

                if (preDefensePassed && normPassed && softwarePassed && plagiarismPassed && hasReview)
                    continue;

                // Build reminder message
                var missing = new List<string>();
                if (!preDefensePassed) missing.Add("предзащита");
                if (!normPassed) missing.Add("нормоконтроль");
                if (!softwarePassed) missing.Add("проверка ПО");
                if (!plagiarismPassed) missing.Add("антиплагиат");
                if (!hasReview) missing.Add("рецензия");

                var body = $"Незавершённые этапы: {string.Join(", ", missing)}.";

                // Send to participants
                var studentUserIds = new List<int>();
                foreach (var participant in work.Participants)
                {
                    var student = await _studentRepository.GetByIdAsync(participant.StudentId, cancellationToken);
                    if (student is not null)
                        studentUserIds.Add(student.UserId);
                }

                if (studentUserIds.Count > 0)
                {
                    await _notificationService.SendToManyAsync(
                        studentUserIds,
                        "Напоминание о готовности к защите",
                        userId.Value,
                        body,
                        relatedEntityType: "StudentWork",
                        relatedEntityId: work.Id,
                        cancellationToken: cancellationToken);
                    notifiedCount += studentUserIds.Count;
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(notifiedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SendReadinessReminders failed for Dept={DeptId}", request.DepartmentId);
            return Result.Failure<int>(new Error("500", ex.Message));
        }
    }
}

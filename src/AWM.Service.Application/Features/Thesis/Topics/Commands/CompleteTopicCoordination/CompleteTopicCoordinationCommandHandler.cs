namespace AWM.Service.Application.Features.Thesis.Topics.Commands.CompleteTopicCoordination;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Services;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Handler for completing topic coordination.
/// Closes all open topics, rejects pending applications, and notifies participants.
/// </summary>
public sealed class CompleteTopicCoordinationCommandHandler
    : IRequestHandler<CompleteTopicCoordinationCommand, Result>
{
    private readonly ITopicRepository _topicRepository;
    private readonly ITopicApplicationRepository _applicationRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompleteTopicCoordinationCommandHandler> _logger;

    public CompleteTopicCoordinationCommandHandler(
        ITopicRepository topicRepository,
        ITopicApplicationRepository applicationRepository,
        IStaffRepository staffRepository,
        IStudentRepository studentRepository,
        INotificationService notificationService,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        ILogger<CompleteTopicCoordinationCommandHandler> logger)
    {
        _topicRepository = topicRepository;
        _applicationRepository = applicationRepository;
        _staffRepository = staffRepository;
        _studentRepository = studentRepository;
        _notificationService = notificationService;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CompleteTopicCoordinationCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.UserId;
        _logger.LogInformation("Completing topic coordination for Dept={DeptId}, Year={YearId} by User={UserId}",
            request.DepartmentId, request.AcademicYearId, userId);

        if (!userId.HasValue)
        {
            return Result.Failure(new Error("401", "User ID is not available."));
        }

        var topics = await _topicRepository.GetByDepartmentAsync(
            request.DepartmentId, request.AcademicYearId, cancellationToken);

        if (topics.Count == 0)
        {
            return Result.Failure(new Error("404", "No topics found for this department and academic year."));
        }

        // Collect Auth.Users.Id sets for notifications.
        // Supervisors: topic.SupervisorId is Staff.Id — resolve to UserId.
        // Students: app.StudentId is Student.Id — resolve to UserId.
        var supervisorUserIds = new HashSet<int>();
        var notifiedStudentUserIds = new HashSet<int>();
        var supervisors = await _staffRepository.GetByIdsAsync(
            topics.Select(t => t.SupervisorId).Distinct(),
            cancellationToken);
        var supervisorsById = supervisors.ToDictionary(s => s.Id);
        var pendingApplications = topics
            .SelectMany(t => t.Applications.Where(a => a.Status == ApplicationStatus.Submitted))
            .ToList();
        var studentsById = (await _studentRepository.GetByIdsAsync(
                pendingApplications.Select(a => a.StudentId).Distinct(),
                cancellationToken))
            .ToDictionary(s => s.Id);

        foreach (var topic in topics)
        {
            if (topic.IsDeleted) continue;

            // Resolve supervisor UserId for notification
            var supervisorStaff = supervisorsById.GetValueOrDefault(topic.SupervisorId);
            if (supervisorStaff is not null)
                supervisorUserIds.Add(supervisorStaff.UserId);
            else
                _logger.LogWarning("CompleteTopicCoordination: Staff not found for StaffId={StaffId}, supervisor notification will be skipped.", topic.SupervisorId);

            // Close all open topics
            if (!topic.IsClosed)
            {
                topic.Close();
                await _topicRepository.UpdateAsync(topic, cancellationToken);
            }

            // Reject all pending applications
            foreach (var app in topic.Applications)
            {
                if (app.Status == ApplicationStatus.Submitted)
                {
                    // ReviewedBy: use JWT userId (admin audit, not a domain Staff FK)
                    app.Reject(userId.Value, "Этап согласования тем завершён.");
                    await _applicationRepository.UpdateAsync(app, cancellationToken);

                    // Resolve student UserId for notification
                    var studentProfile = studentsById.GetValueOrDefault(app.StudentId);
                    if (studentProfile is not null)
                        notifiedStudentUserIds.Add(studentProfile.UserId);
                    else
                        _logger.LogWarning("CompleteTopicCoordination: Student not found for StudentId={StudentId}, student notification will be skipped.", app.StudentId);
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify supervisors (by Auth.Users.Id)
        if (supervisorUserIds.Count > 0)
        {
            await _notificationService.SendToManyAsync(
                userIds: supervisorUserIds,
                title: "Согласование тем завершено",
                createdBy: userId.Value,
                body: "Этап согласования тем завершён. Все темы закрыты для приёма заявок.",
                relatedEntityType: "Department",
                relatedEntityId: request.DepartmentId,
                cancellationToken: cancellationToken);
        }

        // Notify students whose pending applications were rejected (by Auth.Users.Id)
        if (notifiedStudentUserIds.Count > 0)
        {
            await _notificationService.SendToManyAsync(
                userIds: notifiedStudentUserIds,
                title: "Заявка отклонена",
                createdBy: userId.Value,
                body: "Ваша заявка была отклонена в связи с завершением этапа согласования тем.",
                cancellationToken: cancellationToken);
        }

        _logger.LogInformation(
            "Topic coordination completed for Dept={DeptId}. Closed {TopicCount} topics, rejected {StudentCount} pending apps.",
            request.DepartmentId, topics.Count, notifiedStudentUserIds.Count);

        return Result.Success();
    }
}

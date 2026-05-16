namespace AWM.Service.Application.Features.Thesis.Applications.DTOs;

using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;

internal static class TopicApplicationDtoFactory
{
    public static async Task<TopicApplicationDto> CreateAsync(
        TopicApplication application,
        Topic topic,
        IStudentRepository studentRepository,
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        IDirectionRepository directionRepository,
        IWorkflowRepository workflowRepository,
        CancellationToken cancellationToken)
    {
        var student = await studentRepository.GetByIdAsync(application.StudentId, cancellationToken);
        var studentUser = student is null
            ? null
            : await userRepository.GetByIdAsync(student.UserId, cancellationToken);
        var staff = await staffRepository.GetByIdAsync(topic.SupervisorId, cancellationToken);
        var supervisorUser = staff is null
            ? null
            : await userRepository.GetByIdAsync(staff.UserId, cancellationToken);
        var direction = topic.DirectionId.HasValue
            ? await directionRepository.GetByIdAsync(topic.DirectionId.Value, cancellationToken)
            : null;
        var workType = await workflowRepository.GetWorkTypeByIdAsync(topic.WorkTypeId, cancellationToken);

        return new TopicApplicationDto
        {
            Id = application.Id,
            TopicId = application.TopicId,
            StudentId = application.StudentId,
            StudentName = studentUser?.Login ?? studentUser?.Email,
            StudentGroupCode = student?.GroupCode,
            MotivationLetter = application.MotivationLetter,
            AppliedAt = application.AppliedAt,
            Status = application.Status,
            StatusText = application.Status.ToString(),
            ReviewedAt = application.ReviewedAt,
            ReviewedBy = application.ReviewedBy,
            ReviewComment = application.ReviewComment,
            TopicTitleRu = topic.TitleRu,
            TopicTitleKz = topic.TitleKz,
            TopicTitleEn = topic.TitleEn,
            DirectionId = topic.DirectionId,
            DirectionTitleRu = direction?.TitleRu,
            DirectionTitleKz = direction?.TitleKz,
            DirectionTitleEn = direction?.TitleEn,
            SupervisorId = topic.SupervisorId,
            SupervisorName = supervisorUser?.Login ?? supervisorUser?.Email ?? staff?.Position,
            WorkTypeId = topic.WorkTypeId,
            WorkTypeName = workType?.Name,
            TopicMaxParticipants = topic.MaxParticipants,
            TopicAvailableSpots = topic.GetAvailableSpots(),
            IsPending = application.IsPending,
            IsAccepted = application.IsAccepted,
            IsDeleted = application.IsDeleted
        };
    }
}

namespace AWM.Service.Application.Features.Thesis.Topics.DTOs;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Wf.Entities;

internal static class TopicDtoFactory
{
    public static TopicDto Create(
        Topic topic,
        Direction? direction,
        Staff? supervisorStaff,
        User? supervisorUser,
        WorkType? workType)
    {
        var supervisorName = supervisorUser?.Login ?? supervisorUser?.Email ?? supervisorStaff?.Position;
        var applications = topic.Applications.Where(a => !a.IsDeleted).ToList();

        return new TopicDto
        {
            Id = topic.Id,
            DirectionId = topic.DirectionId,
            DepartmentId = topic.DepartmentId,
            SupervisorId = topic.SupervisorId,
            AcademicYearId = topic.AcademicYearId,
            WorkTypeId = topic.WorkTypeId,
            TitleRu = topic.TitleRu,
            TitleEn = topic.TitleEn,
            TitleKz = topic.TitleKz,
            DescriptionRu = topic.DescriptionRu,
            DescriptionKz = topic.DescriptionKz,
            DescriptionEn = topic.DescriptionEn,
            DirectionTitleRu = direction?.TitleRu,
            DirectionTitleKz = direction?.TitleKz,
            DirectionTitleEn = direction?.TitleEn,
            SupervisorName = supervisorName,
            WorkTypeName = workType?.Name,
            MaxParticipants = topic.MaxParticipants,
            AvailableSpots = topic.GetAvailableSpots(),
            AcceptedApplicationsCount = applications.Count(a => a.Status == ApplicationStatus.Accepted),
            PendingApplicationsCount = applications.Count(a => a.Status == ApplicationStatus.Submitted),
            ApplicationsCount = applications.Count,
            IsSubmittedForApproval = topic.IsSubmittedForApproval,
            IsApproved = topic.IsApproved,
            IsClosed = topic.IsClosed,
            IsTeamTopic = topic.IsTeamTopic,
            CreatedAt = topic.CreatedAt
        };
    }

    public static async Task<TopicDto> CreateAsync(
        Topic topic,
        IDirectionRepository directionRepository,
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        IWorkflowRepository workflowRepository,
        CancellationToken cancellationToken)
    {
        var direction = topic.DirectionId.HasValue
            ? await directionRepository.GetByIdAsync(topic.DirectionId.Value, cancellationToken)
            : null;
        var staff = await staffRepository.GetByIdAsync(topic.SupervisorId, cancellationToken);
        var user = staff is null ? null : await userRepository.GetByIdAsync(staff.UserId, cancellationToken);
        var workType = await workflowRepository.GetWorkTypeByIdAsync(topic.WorkTypeId, cancellationToken);

        return Create(topic, direction, staff, user, workType);
    }

    public static async Task<TopicDetailDto> CreateDetailAsync(
        Topic topic,
        IDirectionRepository directionRepository,
        IStaffRepository staffRepository,
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        IWorkflowRepository workflowRepository,
        CancellationToken cancellationToken)
    {
        var topicDto = await CreateAsync(
            topic,
            directionRepository,
            staffRepository,
            userRepository,
            workflowRepository,
            cancellationToken);

        var applications = new List<TopicApplicationDto>();
        foreach (var application in topic.Applications.Where(a => !a.IsDeleted).OrderByDescending(a => a.AppliedAt))
        {
            var student = await studentRepository.GetByIdAsync(application.StudentId, cancellationToken);
            var user = student is null
                ? null
                : await userRepository.GetByIdAsync(student.UserId, cancellationToken);

            applications.Add(new TopicApplicationDto
            {
                Id = application.Id,
                TopicId = application.TopicId,
                StudentId = application.StudentId,
                StudentName = user?.Login,
                StudentGroupCode = student?.GroupCode,
                MotivationLetter = application.MotivationLetter,
                Status = application.Status.ToString(),
                AppliedAt = application.AppliedAt,
                ReviewedAt = application.ReviewedAt,
                ReviewedBy = application.ReviewedBy,
                ReviewComment = application.ReviewComment
            });
        }

        return new TopicDetailDto
        {
            Id = topicDto.Id,
            DirectionId = topicDto.DirectionId,
            DepartmentId = topicDto.DepartmentId,
            SupervisorId = topicDto.SupervisorId,
            AcademicYearId = topicDto.AcademicYearId,
            WorkTypeId = topicDto.WorkTypeId,
            TitleRu = topicDto.TitleRu,
            TitleEn = topicDto.TitleEn,
            TitleKz = topicDto.TitleKz,
            DescriptionRu = topicDto.DescriptionRu,
            DescriptionKz = topicDto.DescriptionKz,
            DescriptionEn = topicDto.DescriptionEn,
            DirectionTitleRu = topicDto.DirectionTitleRu,
            DirectionTitleKz = topicDto.DirectionTitleKz,
            DirectionTitleEn = topicDto.DirectionTitleEn,
            SupervisorName = topicDto.SupervisorName,
            WorkTypeName = topicDto.WorkTypeName,
            MaxParticipants = topicDto.MaxParticipants,
            AvailableSpots = topicDto.AvailableSpots,
            AcceptedApplicationsCount = topicDto.AcceptedApplicationsCount,
            PendingApplicationsCount = topicDto.PendingApplicationsCount,
            ApplicationsCount = topicDto.ApplicationsCount,
            IsSubmittedForApproval = topicDto.IsSubmittedForApproval,
            IsApproved = topicDto.IsApproved,
            IsClosed = topicDto.IsClosed,
            IsTeamTopic = topicDto.IsTeamTopic,
            CreatedAt = topicDto.CreatedAt,
            CreatedBy = topic.CreatedBy,
            LastModifiedAt = topic.LastModifiedAt,
            LastModifiedBy = topic.LastModifiedBy,
            Applications = applications
        };
    }
}
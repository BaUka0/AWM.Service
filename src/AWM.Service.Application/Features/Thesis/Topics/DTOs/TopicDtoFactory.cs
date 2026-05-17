namespace AWM.Service.Application.Features.Thesis.Topics.DTOs;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Wf.Entities;
using ApplicationTopicApplicationDto = AWM.Service.Application.Features.Thesis.Applications.DTOs.TopicApplicationDto;

internal readonly record struct TopicApplicationCounters(
    int ApplicationsCount,
    int PendingApplicationsCount,
    int AcceptedApplicationsCount)
{
    public static TopicApplicationCounters Empty => new(0, 0, 0);

    public static TopicApplicationCounters FromApplications(IEnumerable<TopicApplication> applications)
    {
        var applicationsCount = 0;
        var pendingApplicationsCount = 0;
        var acceptedApplicationsCount = 0;

        foreach (var application in applications)
        {
            if (application.IsDeleted)
            {
                continue;
            }

            applicationsCount++;

            if (application.Status == ApplicationStatus.Submitted)
            {
                pendingApplicationsCount++;
            }
            else if (application.Status == ApplicationStatus.Accepted)
            {
                acceptedApplicationsCount++;
            }
        }

        return new TopicApplicationCounters(applicationsCount, pendingApplicationsCount, acceptedApplicationsCount);
    }
}

internal static class TopicDtoFactory
{
    public static TopicDto Create(
        Topic topic,
        Direction? direction,
        Staff? supervisorStaff,
        User? supervisorUser,
        WorkType? workType,
        TopicApplicationCounters applicationCounters)
    {
        var supervisorName = supervisorUser?.Login ?? supervisorUser?.Email ?? supervisorStaff?.Position;
        var availableSpots = Math.Max(0, topic.MaxParticipants - applicationCounters.AcceptedApplicationsCount);

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
            AvailableSpots = availableSpots,
            AcceptedApplicationsCount = applicationCounters.AcceptedApplicationsCount,
            PendingApplicationsCount = applicationCounters.PendingApplicationsCount,
            ApplicationsCount = applicationCounters.ApplicationsCount,
            IsSubmittedForApproval = topic.IsSubmittedForApproval,
            IsApproved = topic.IsApproved,
            IsClosed = topic.IsClosed,
            IsTeamTopic = topic.IsTeamTopic,
            CreatedAt = topic.CreatedAt
        };
    }

    public static TopicDetailDto CreateDetail(
        Topic topic,
        Direction? direction,
        Staff? supervisorStaff,
        User? supervisorUser,
        WorkType? workType,
        TopicApplicationCounters applicationCounters,
        IReadOnlyCollection<ApplicationTopicApplicationDto> applications)
    {
        var topicDto = Create(
            topic,
            direction,
            supervisorStaff,
            supervisorUser,
            workType,
            applicationCounters);

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

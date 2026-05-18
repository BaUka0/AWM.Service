namespace AWM.Service.Application.Features.Thesis.Applications.DTOs;

using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Wf.Entities;

internal static class TopicApplicationDtoFactory
{
    public static TopicApplicationDto Create(
        TopicApplication application,
        Topic topic,
        Student? student,
        User? studentUser,
        Staff? supervisorStaff,
        User? supervisorUser,
        Direction? direction,
        WorkType? workType,
        int availableSpots)
    {
        return new TopicApplicationDto
        {
            Id = application.Id,
            TopicId = application.TopicId,
            StudentId = application.StudentId,
            StudentName = studentUser?.Login ?? studentUser?.Email,
            StudentGroupCode = student?.GroupCode,
            MotivationLetter = application.MotivationLetter,
            AppliedAt = application.AppliedAt,
            StatusId = application.StatusId,
            StatusText = application.StatusId.ToString(),
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
            SupervisorName = supervisorUser?.Login ?? supervisorUser?.Email ?? supervisorStaff?.Position,
            WorkTypeId = topic.WorkTypeId,
            WorkTypeName = workType?.Name,
            TopicMaxParticipants = topic.MaxParticipants,
            TopicAvailableSpots = availableSpots,
            IsPending = application.IsPending,
            IsAccepted = application.IsAccepted,
            IsDeleted = application.IsDeleted
        };
    }
}

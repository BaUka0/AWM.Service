using AWM.Service.Application.Features.Workflow.Reviews.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Reviews.Queries.GetMyReviewerAssignments;

public sealed class GetMyReviewerAssignmentsQueryHandler : IRequestHandler<GetMyReviewerAssignmentsQuery, Result<IReadOnlyList<ReviewerAssignmentDto>>>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetMyReviewerAssignmentsQueryHandler(
        IStaffAssignmentRepository staffAssignmentRepository,
        IStudentWorkRepository studentWorkRepository,
        ITopicRepository topicRepository,
        IUserRepository userRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
        _studentWorkRepository = studentWorkRepository;
        _topicRepository = topicRepository;
        _userRepository = userRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<IReadOnlyList<ReviewerAssignmentDto>>> Handle(GetMyReviewerAssignmentsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<ReviewerAssignmentDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var staffAssignments = await _staffAssignmentRepository.GetByUserAsync(currentUserId, cancellationToken);
        var activeAssignments = staffAssignments
            .Where(a => a.RoleType == StaffRoleType.Reviewer &&
                        a.TargetEntityType == "StudentWork" &&
                        a.IsActive &&
                        !a.IsDeleted)
            .ToList();

        if (!activeAssignments.Any())
        {
            return Result.Success<IReadOnlyList<ReviewerAssignmentDto>>(Array.Empty<ReviewerAssignmentDto>());
        }

        var workIds = activeAssignments.Select(a => a.TargetEntityId).Distinct().ToList();
        var works = await _studentWorkRepository.GetByIdsWithDetailsAsync(workIds, cancellationToken);

        var topicIds = works.Where(w => w.TopicId.HasValue).Select(w => w.TopicId!.Value).Distinct().ToList();
        var topics = await _topicRepository.GetByIdsAsync(topicIds, cancellationToken);
        var topicMap = topics.ToDictionary(t => t.Id);

        var studentIds = works.SelectMany(w => w.Participants.Select(p => p.StudentId)).Distinct().ToList();
        var students = await _userRepository.GetByIdsAsync(studentIds, cancellationToken);
        var studentMap = students.ToDictionary(s => s.Id);

        var dtos = new List<ReviewerAssignmentDto>();
        foreach (var assignment in activeAssignments)
        {
            var workId = assignment.TargetEntityId;
            var work = works.FirstOrDefault(w => w.Id == workId);
            if (work == null) continue;

            string topicTitle = string.Empty;
            if (work.TopicId.HasValue && topicMap.TryGetValue(work.TopicId.Value, out var topic))
            {
                topicTitle = topic.TitleRu ?? topic.TitleEn ?? topic.TitleKz ?? string.Empty;
            }

            string studentName = "Unknown";
            var participant = work.Participants.FirstOrDefault();
            if (participant != null && studentMap.TryGetValue(participant.StudentId, out var studentUser))
            {
                studentName = $"{studentUser.LastName} {studentUser.FirstName} {studentUser.MiddleName}".Trim();
            }

            var externalReview = work.WorkReviews.FirstOrDefault(r => r.Type == ReviewType.ExternalReview);
            var isReviewUploaded = externalReview != null;
            var reviewId = externalReview?.Id;

            dtos.Add(new ReviewerAssignmentDto(
                workId,
                isReviewUploaded,
                topicTitle,
                studentName,
                reviewId));
        }

        return Result.Success<IReadOnlyList<ReviewerAssignmentDto>>(dtos);
    }
}

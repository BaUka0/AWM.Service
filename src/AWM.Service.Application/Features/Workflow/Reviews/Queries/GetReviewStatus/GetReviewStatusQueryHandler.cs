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

namespace AWM.Service.Application.Features.Workflow.Reviews.Queries.GetReviewStatus;

public sealed class GetReviewStatusQueryHandler : IRequestHandler<GetReviewStatusQuery, Result<IReadOnlyList<WorkReviewStatusDto>>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetReviewStatusQueryHandler(
        IStudentWorkRepository studentWorkRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        ITopicRepository topicRepository,
        IUserRepository userRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _studentWorkRepository = studentWorkRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
        _topicRepository = topicRepository;
        _userRepository = userRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<IReadOnlyList<WorkReviewStatusDto>>> Handle(GetReviewStatusQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<WorkReviewStatusDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        // Get basic works in department
        var basicWorks = await _studentWorkRepository.GetByOrgUnitAsync(request.OrgUnitId, request.SemesterId, cancellationToken);
        if (!basicWorks.Any())
        {
            return Result.Success<IReadOnlyList<WorkReviewStatusDto>>(Array.Empty<WorkReviewStatusDto>());
        }

        var workIds = basicWorks.Select(w => w.Id).ToList();
        var works = await _studentWorkRepository.GetByIdsWithDetailsAsync(workIds, cancellationToken);

        // Load Topics
        var topicIds = works.Where(w => w.TopicId.HasValue).Select(w => w.TopicId!.Value).Distinct().ToList();
        var topics = await _topicRepository.GetByIdsAsync(topicIds, cancellationToken);
        var topicMap = topics.ToDictionary(t => t.Id);

        // Load Reviewer Assignments
        var reviewerAssignments = await _staffAssignmentRepository.GetByTargetsAndRoleAsync("StudentWork", workIds, StaffRoleType.Reviewer, cancellationToken);
        var reviewerAssignmentsMap = reviewerAssignments.ToLookup(a => a.TargetEntityId);

        // Resolve all user names in bulk
        var studentIds = works.SelectMany(w => w.Participants.Select(p => p.StudentId)).Distinct().ToList();
        var supervisorIds = topics.Select(t => t.CreatedBy).Distinct().ToList();
        var reviewerIds = reviewerAssignments.Select(a => a.UserId).Distinct().ToList();
        var allUserIds = studentIds.Concat(supervisorIds).Concat(reviewerIds).Distinct().ToList();

        var users = await _userRepository.GetByIdsAsync(allUserIds, cancellationToken);
        var userMap = users.ToDictionary(u => u.Id);

        var dtos = new List<WorkReviewStatusDto>();

        foreach (var work in works)
        {
            // Resolve Student Name
            string studentName = "Unknown";
            var participant = work.Participants.FirstOrDefault();
            if (participant != null && userMap.TryGetValue(participant.StudentId, out var studentUser))
            {
                studentName = $"{studentUser.LastName} {studentUser.FirstName} {studentUser.MiddleName}".Trim();
            }

            // Resolve Topic & Supervisor Name
            string topicTitle = string.Empty;
            string supervisorName = "Unknown";
            if (work.TopicId.HasValue && topicMap.TryGetValue(work.TopicId.Value, out var topic))
            {
                topicTitle = topic.TitleRu ?? topic.TitleEn ?? topic.TitleKz ?? string.Empty;
                if (userMap.TryGetValue(topic.CreatedBy, out var supervisorUser))
                {
                    supervisorName = $"{supervisorUser.LastName} {supervisorUser.FirstName} {supervisorUser.MiddleName}".Trim();
                }
            }

            // Resolve Reviewer Name
            string reviewerName = "Not Assigned";
            var activeReviewerAssignment = reviewerAssignmentsMap[work.Id].FirstOrDefault();
            if (activeReviewerAssignment != null && userMap.TryGetValue(activeReviewerAssignment.UserId, out var reviewerUser))
            {
                reviewerName = $"{reviewerUser.LastName} {reviewerUser.FirstName} {reviewerUser.MiddleName}".Trim();
            }

            var isSupervisorReviewSubmitted = work.WorkReviews.Any(r => r.Type == ReviewType.SupervisorReview);
            var isReviewerReviewSubmitted = work.WorkReviews.Any(r => r.Type == ReviewType.ExternalReview);

            dtos.Add(new WorkReviewStatusDto(
                work.Id,
                studentName,
                topicTitle,
                supervisorName,
                reviewerName,
                isSupervisorReviewSubmitted,
                isReviewerReviewSubmitted));
        }

        return Result.Success<IReadOnlyList<WorkReviewStatusDto>>(dtos);
    }
}

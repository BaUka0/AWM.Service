namespace AWM.Service.Application.Features.Thesis.Reviews.Queries.GetMyReviewerAssignments;

using AWM.Service.Application.Features.Thesis.Reviews.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetMyReviewerAssignmentsQueryHandler
    : IRequestHandler<GetMyReviewerAssignmentsQuery, Result<IReadOnlyList<ReviewerAssignmentDto>>>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IStudentWorkRepository _workRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationLookupRepository _orgLookupRepository;

    public GetMyReviewerAssignmentsQueryHandler(
        ICurrentUserProvider currentUserProvider,
        IReviewerRepository reviewerRepository,
        IReviewRepository reviewRepository,
        IStudentWorkRepository workRepository,
        ITopicRepository topicRepository,
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        IOrganizationLookupRepository orgLookupRepository)
    {
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
        _reviewerRepository = reviewerRepository ?? throw new ArgumentNullException(nameof(reviewerRepository));
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _orgLookupRepository = orgLookupRepository ?? throw new ArgumentNullException(nameof(orgLookupRepository));
    }

    public async Task<Result<IReadOnlyList<ReviewerAssignmentDto>>> Handle(
        GetMyReviewerAssignmentsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<ReviewerAssignmentDto>>(
                new Error("Authorization.Unauthorized", "User identity could not be determined."));
        }

        var userId = _currentUserProvider.UserId.Value;

        var reviewer = await _reviewerRepository.GetByUserIdAsync(userId, cancellationToken);
        if (reviewer is null)
        {
            return Result.Success<IReadOnlyList<ReviewerAssignmentDto>>([]);
        }

        var reviews = await _reviewRepository.GetByReviewerAsync(reviewer.Id, cancellationToken);
        var works = await _workRepository.GetByIdsAsync(reviews.Select(r => r.WorkId).Distinct(), cancellationToken);
        var worksById = works.ToDictionary(w => w.Id);
        var topics = await _topicRepository.GetByIdsAsync(
            works.Where(w => w.TopicId.HasValue).Select(w => w.TopicId!.Value).Distinct(),
            cancellationToken);
        var topicsById = topics.ToDictionary(t => t.Id);
        var leaderStudentIds = works
            .Select(w => w.Participants.FirstOrDefault(p => p.IsLeader)?.StudentId)
            .Where(studentId => studentId.HasValue)
            .Select(studentId => studentId!.Value)
            .Distinct()
            .ToList();
        var students = await _studentRepository.GetByIdsAsync(leaderStudentIds, cancellationToken);
        var studentsById = students.ToDictionary(s => s.Id);
        var users = await _userRepository.GetByIdsAsync(
            students.Select(s => s.UserId).Distinct(),
            cancellationToken);
        var usersById = users.ToDictionary(u => u.Id);
        var departments = await _orgLookupRepository.GetDepartmentsByIdsAsync(
            works.Where(w => w.OrgUnitId > 0).Select(w => w.OrgUnitId).Distinct(),
            cancellationToken);
        var departmentsById = departments.ToDictionary(d => d.Id);

        var dtos = new List<ReviewerAssignmentDto>();

        foreach (var review in reviews)
        {
            if (!worksById.TryGetValue(review.WorkId, out var work))
                continue;

            var topic = work.TopicId.HasValue
                ? topicsById.GetValueOrDefault(work.TopicId.Value)
                : null;

            string? studentName = null;
            var leader = work.Participants.FirstOrDefault(p => p.IsLeader);
            if (leader is not null)
            {
                var student = studentsById.GetValueOrDefault(leader.StudentId);
                var studentUser = student is not null
                    ? usersById.GetValueOrDefault(student.UserId)
                    : null;
                studentName = studentUser?.Email ?? studentUser?.FirstName;
            }

            string? departmentName = null;
            if (work.OrgUnitId > 0)
            {
                departmentName = departmentsById.GetValueOrDefault(work.OrgUnitId)?.Title;
            }

            dtos.Add(new ReviewerAssignmentDto
            {
                WorkId = work.Id,
                ReviewId = review.Id,
                TopicTitle = topic is not null ? topic.TitleRu : null,
                StudentName = studentName,
                OrgUnitId = work.OrgUnitId,
                DepartmentName = departmentName,
                IsReviewUploaded = review.IsUploaded,
                AssignedAt = review.CreatedAt,
                UploadedAt = review.UploadedAt
            });
        }

        return Result.Success<IReadOnlyList<ReviewerAssignmentDto>>(dtos);
    }
}

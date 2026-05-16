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

        var dtos = new List<ReviewerAssignmentDto>();

        foreach (var review in reviews)
        {
            var work = await _workRepository.GetByIdWithDetailsAsync(review.WorkId, cancellationToken);
            if (work is null)
                continue;

            var topic = work.TopicId.HasValue
                ? await _topicRepository.GetByIdAsync(work.TopicId.Value, cancellationToken)
                : null;

            string? studentName = null;
            var leader = work.Participants.FirstOrDefault(p => p.IsLeader);
            if (leader is not null)
            {
                var student = await _studentRepository.GetByIdAsync(leader.StudentId, cancellationToken);
                var studentUser = student is not null
                    ? await _userRepository.GetByIdAsync(student.UserId, cancellationToken)
                    : null;
                studentName = studentUser?.Login ?? studentUser?.Email;
            }

            string? departmentName = null;
            if (work.DepartmentId > 0)
            {
                var dept = await _orgLookupRepository.GetDepartmentByIdAsync(work.DepartmentId, cancellationToken);
                departmentName = dept?.Name;
            }

            dtos.Add(new ReviewerAssignmentDto
            {
                WorkId = work.Id,
                ReviewId = review.Id,
                TopicTitle = topic is not null ? topic.TitleRu : null,
                StudentName = studentName,
                DepartmentId = work.DepartmentId,
                DepartmentName = departmentName,
                IsReviewUploaded = review.IsUploaded,
                AssignedAt = review.CreatedAt,
                UploadedAt = review.UploadedAt
            });
        }

        return Result.Success<IReadOnlyList<ReviewerAssignmentDto>>(dtos);
    }
}

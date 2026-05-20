namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetReviewStatusByDepartment;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetReviewStatusByDepartmentQueryHandler
    : IRequestHandler<GetReviewStatusByDepartmentQuery, Result<ReviewStatusByDepartmentDto>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetReviewStatusByDepartmentQueryHandler(
        IStudentWorkRepository workRepository,
        IReviewRepository reviewRepository,
        IReviewerRepository reviewerRepository,
        ITopicRepository topicRepository,
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _reviewerRepository = reviewerRepository ?? throw new ArgumentNullException(nameof(reviewerRepository));
        _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<ReviewStatusByDepartmentDto>> Handle(
        GetReviewStatusByDepartmentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<ReviewStatusByDepartmentDto>(new Error("401", "User ID is not available."));

            var works = await _workRepository.GetByDepartmentAsync(
                request.DepartmentId, request.AcademicYearId, cancellationToken);

            var items = new List<WorkReviewStatusItem>();

            // Bulk fetch all reviews for works in this department (avoids N+1)
            var workIds = works.Select(w => w.Id).ToList();
            var allReviews = await _reviewRepository.GetByWorkIdsAsync(workIds, cancellationToken);
            var reviewsByWorkId = allReviews.GroupBy(r => r.WorkId)
                .ToDictionary(g => g.Key, g => g.First());

            // Bulk fetch all relevant reviewers
            var reviewerIds = reviewsByWorkId.Values
                .Select(r => r.ReviewerId)
                .Distinct()
                .ToList();
            var allReviewers = await _reviewerRepository.GetByIdsAsync(reviewerIds, cancellationToken);
            var reviewersById = allReviewers.ToDictionary(r => r.Id);

            // Bulk fetch topics
            var topicIds = works.Where(w => w.TopicId.HasValue).Select(w => w.TopicId!.Value).Distinct().ToList();
            var allTopics = await _topicRepository.GetByIdsAsync(topicIds, cancellationToken);
            var topicsById = allTopics.ToDictionary(t => t.Id);

            // Bulk fetch participants / students
            var participantStudentIds = works
                .SelectMany(w => w.Participants.Take(1).Select(p => p.StudentId))
                .Distinct()
                .ToList();
            var allStudents = await _studentRepository.GetByIdsAsync(participantStudentIds, cancellationToken);
            var studentsById = allStudents.ToDictionary(s => s.Id);

            // Bulk fetch users for student names
            var userIds = allStudents.Select(s => s.Id).Distinct().ToList();
            var allUsers = await _userRepository.GetByIdsAsync(userIds, cancellationToken);
            var usersById = allUsers.ToDictionary(u => u.Id);

            foreach (var work in works)
            {
                reviewsByWorkId.TryGetValue(work.Id, out var review);

                string? reviewerName = null;
                if (review is not null && reviewersById.TryGetValue(review.ReviewerId, out var reviewer))
                {
                    reviewerName = reviewer.FullName;
                }

                string? topicTitle = null;
                if (work.TopicId.HasValue && topicsById.TryGetValue(work.TopicId.Value, out var topic))
                {
                    topicTitle = topic.TitleRu;
                }

                string? studentName = null;
                var participant = work.Participants.FirstOrDefault();
                if (participant is not null && studentsById.TryGetValue(participant.StudentId, out var student))
                {
                    if (usersById.TryGetValue(student.Id, out var studentUser))
                    {
                        studentName = studentUser.Email ?? studentUser.FirstName;
                    }
                }

                items.Add(new WorkReviewStatusItem
                {
                    WorkId = work.Id,
                    TopicTitle = topicTitle,
                    StudentName = studentName,
                    ReviewerId = review?.ReviewerId,
                    ReviewerName = reviewerName,
                    HasReviewer = review is not null,
                    IsReviewUploaded = review?.IsUploaded ?? false
                });
            }

            var dto = new ReviewStatusByDepartmentDto
            {
                TotalWorks = items.Count,
                WorksWithReviewer = items.Count(i => i.HasReviewer),
                WorksWithoutReviewer = items.Count(i => !i.HasReviewer),
                ReviewsUploaded = items.Count(i => i.IsReviewUploaded),
                ReviewsPending = items.Count(i => i.HasReviewer && !i.IsReviewUploaded),
                Items = items
            };

            return Result.Success(dto);
        }
        catch (Exception ex)
        {
            return Result.Failure<ReviewStatusByDepartmentDto>(new Error("500", ex.Message));
        }
    }
}

namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetAdmittedStudents;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetAdmittedStudentsQueryHandler
    : IRequestHandler<GetAdmittedStudentsQuery, Result<IReadOnlyList<AdmittedStudentDto>>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IPreDefenseAttemptRepository _attemptRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetAdmittedStudentsQueryHandler(
        IStudentWorkRepository workRepository,
        IPreDefenseAttemptRepository attemptRepository,
        IReviewRepository reviewRepository,
        IStudentRepository studentRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<IReadOnlyList<AdmittedStudentDto>>> Handle(
        GetAdmittedStudentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<IReadOnlyList<AdmittedStudentDto>>(
                    new Error("401", "User ID is not available."));

            var works = await _workRepository.GetByDepartmentWithParticipantsAndQualityChecksAsync(
                request.DepartmentId, request.AcademicYearId, cancellationToken);
            var workIds = works.Select(w => w.Id).ToList();
            var attempts = await _attemptRepository.GetByWorkIdsAsync(workIds, cancellationToken);
            var attemptsByWorkId = attempts.GroupBy(a => a.WorkId)
                .ToDictionary(g => g.Key, g => g.ToList());
            var reviews = await _reviewRepository.GetByWorkIdsAsync(workIds, cancellationToken);
            var reviewsByWorkId = reviews.GroupBy(r => r.WorkId)
                .ToDictionary(g => g.Key, g => g.ToList());
            var students = await _studentRepository.GetByIdsAsync(
                works.SelectMany(w => w.Participants.Select(p => p.StudentId)).Distinct(),
                cancellationToken);
            var studentsById = students.ToDictionary(s => s.Id);

            var admitted = new List<AdmittedStudentDto>();

            foreach (var work in works)
            {
                var workAttempts = attemptsByWorkId.GetValueOrDefault(work.Id, []);
                if (!workAttempts.Any(a => a.IsPassed)) continue;
                if (!work.HasPassedCheck(1)) continue;
                if (!work.HasPassedCheck(2)) continue;
                if (!work.HasPassedCheck(3)) continue;

                var workReviews = reviewsByWorkId.GetValueOrDefault(work.Id, []);
                if (!workReviews.Any(r => r.IsUploaded)) continue;

                // All checks passed — student is admitted
                var leader = work.GetLeader();
                if (leader is null) continue;

                var student = studentsById.GetValueOrDefault(leader.StudentId);
                if (student is null) continue;

                admitted.Add(new AdmittedStudentDto
                {
                    WorkId = work.Id,
                    StudentId = student.Id,
                    UserId = student.UserId
                });
            }

            return Result.Success<IReadOnlyList<AdmittedStudentDto>>(admitted);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<AdmittedStudentDto>>(new Error("500", ex.Message));
        }
    }
}

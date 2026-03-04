namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetAdmittedStudents;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
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

            var works = await _workRepository.GetByDepartmentAsync(
                request.DepartmentId, request.AcademicYearId, cancellationToken);

            var admitted = new List<AdmittedStudentDto>();

            foreach (var work in works)
            {
                var attempts = await _attemptRepository.GetByWorkIdAsync(work.Id, cancellationToken);
                if (!attempts.Any(a => a.IsPassed)) continue;
                if (!work.HasPassedCheck(CheckType.NormControl)) continue;
                if (!work.HasPassedCheck(CheckType.SoftwareCheck)) continue;
                if (!work.HasPassedCheck(CheckType.AntiPlagiarism)) continue;

                var reviews = await _reviewRepository.GetByWorkIdAsync(work.Id, cancellationToken);
                if (!reviews.Any(r => r.IsUploaded)) continue;

                // All checks passed — student is admitted
                var leader = work.GetLeader();
                if (leader is null) continue;

                var student = await _studentRepository.GetByIdAsync(leader.StudentId, cancellationToken);
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

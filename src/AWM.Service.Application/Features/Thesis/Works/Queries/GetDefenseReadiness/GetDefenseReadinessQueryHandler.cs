namespace AWM.Service.Application.Features.Thesis.Works.Queries.GetDefenseReadiness;

using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Enums;
using KDS.Primitives.FluentResult;
using MediatR;

public sealed class GetDefenseReadinessQueryHandler
    : IRequestHandler<GetDefenseReadinessQuery, Result<DefenseReadinessDto>>
{
    private readonly IStudentWorkRepository _workRepository;
    private readonly IPreDefenseAttemptRepository _attemptRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetDefenseReadinessQueryHandler(
        IStudentWorkRepository workRepository,
        IPreDefenseAttemptRepository attemptRepository,
        IReviewRepository reviewRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _workRepository = workRepository ?? throw new ArgumentNullException(nameof(workRepository));
        _attemptRepository = attemptRepository ?? throw new ArgumentNullException(nameof(attemptRepository));
        _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
        _currentUserProvider = currentUserProvider ?? throw new ArgumentNullException(nameof(currentUserProvider));
    }

    public async Task<Result<DefenseReadinessDto>> Handle(
        GetDefenseReadinessQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _currentUserProvider.UserId;
            if (!userId.HasValue)
                return Result.Failure<DefenseReadinessDto>(new Error("401", "User ID is not available."));

            var works = await _workRepository.GetByDepartmentAsync(
                request.DepartmentId, request.AcademicYearId, cancellationToken);

            var items = new List<StudentReadinessItem>();

            foreach (var work in works)
            {
                var attempts = await _attemptRepository.GetByWorkIdAsync(work.Id, cancellationToken);
                var preDefensePassed = attempts.Any(a => a.IsPassed);

                var normPassed = work.HasPassedCheck(CheckType.NormControl);
                var softwarePassed = work.HasPassedCheck(CheckType.SoftwareCheck);
                var plagiarismPassed = work.HasPassedCheck(CheckType.AntiPlagiarism);

                var reviews = await _reviewRepository.GetByWorkIdAsync(work.Id, cancellationToken);
                var hasReview = reviews.Any(r => r.IsUploaded);

                var isReady = preDefensePassed && normPassed && softwarePassed && plagiarismPassed && hasReview;

                items.Add(new StudentReadinessItem
                {
                    WorkId = work.Id,
                    PreDefensePassed = preDefensePassed,
                    NormControlPassed = normPassed,
                    SoftwareCheckPassed = softwarePassed,
                    AntiPlagiarismPassed = plagiarismPassed,
                    HasReview = hasReview,
                    IsFullyReady = isReady
                });
            }

            return Result.Success(new DefenseReadinessDto
            {
                TotalWorks = items.Count,
                FullyReady = items.Count(i => i.IsFullyReady),
                NotReady = items.Count(i => !i.IsFullyReady),
                Items = items
            });
        }
        catch (Exception ex)
        {
            return Result.Failure<DefenseReadinessDto>(new Error("500", ex.Message));
        }
    }
}

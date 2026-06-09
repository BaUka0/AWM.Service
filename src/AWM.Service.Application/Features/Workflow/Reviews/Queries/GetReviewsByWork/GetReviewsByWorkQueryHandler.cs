using AWM.Service.Application.Features.Workflow.Reviews.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Reviews.Queries.GetReviewsByWork;

public sealed class GetReviewsByWorkQueryHandler : IRequestHandler<GetReviewsByWorkQuery, Result<IReadOnlyList<WorkReviewDto>>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetReviewsByWorkQueryHandler(
        IStudentWorkRepository studentWorkRepository,
        IUserRepository userRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _studentWorkRepository = studentWorkRepository;
        _userRepository = userRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<IReadOnlyList<WorkReviewDto>>> Handle(GetReviewsByWorkQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<WorkReviewDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure<IReadOnlyList<WorkReviewDto>>(new Error("StudentWorks.NotFound", $"Student work with ID {request.WorkId} not found."));
        }

        if (!work.WorkReviews.Any())
        {
            return Result.Success<IReadOnlyList<WorkReviewDto>>(new List<WorkReviewDto>());
        }

        var authorIds = work.WorkReviews.Select(r => r.AuthorUserId).Distinct().ToList();
        var authors = await _userRepository.GetByIdsAsync(authorIds, cancellationToken);
        var authorMap = authors.ToDictionary(a => a.Id);

        var dtos = work.WorkReviews.Select(r =>
        {
            string authorName = "Unknown";
            if (authorMap.TryGetValue(r.AuthorUserId, out var user))
            {
                authorName = $"{user.LastName} {user.FirstName} {user.MiddleName}".Trim();
            }

            return new WorkReviewDto(
                r.Id,
                r.WorkId,
                r.AuthorUserId,
                authorName,
                r.Type,
                r.ReviewText,
                r.MetadataJson,
                r.IsFinal,
                r.CreatedAt);
        }).ToList();

        return Result.Success<IReadOnlyList<WorkReviewDto>>(dtos);
    }
}

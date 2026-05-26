using AWM.Service.Application.Features.Workflow.Reviewers.DTOs;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Entities;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Reviewers.Queries.GetReviewers;

public sealed class GetReviewersQueryHandler : IRequestHandler<GetReviewersQuery, Result<IReadOnlyList<ReviewerDto>>>
{
    private readonly IReviewerRepository _reviewerRepository;

    public GetReviewersQueryHandler(IReviewerRepository reviewerRepository)
    {
        _reviewerRepository = reviewerRepository;
    }

    public async Task<Result<IReadOnlyList<ReviewerDto>>> Handle(GetReviewersQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Reviewer> reviewers;

        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            reviewers = await _reviewerRepository.GetActiveAsync(cancellationToken);
        }
        else
        {
            reviewers = await _reviewerRepository.SearchAsync(request.SearchTerm, cancellationToken);
        }

        var dtos = reviewers.Select(r => new ReviewerDto(
            r.Id,
            r.FullName,
            r.Position,
            r.AcademicDegree,
            r.Organization,
            r.Email,
            r.Phone,
            r.IsActive,
            r.UserId
        )).ToList();

        return Result.Success<IReadOnlyList<ReviewerDto>>(dtos);
    }
}

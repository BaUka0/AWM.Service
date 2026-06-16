using AWM.Service.Application.Features.Workflow.Reviewers.DTOs;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Reviewers.Queries.GetAssignedReviewer;

public sealed class GetAssignedReviewerQueryHandler : IRequestHandler<GetAssignedReviewerQuery, Result<ReviewerDto?>>
{
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IReviewerRepository _reviewerRepository;

    public GetAssignedReviewerQueryHandler(
        IStaffAssignmentRepository staffAssignmentRepository,
        IReviewerRepository reviewerRepository)
    {
        _staffAssignmentRepository = staffAssignmentRepository;
        _reviewerRepository = reviewerRepository;
    }

    public async Task<Result<ReviewerDto?>> Handle(GetAssignedReviewerQuery request, CancellationToken cancellationToken)
    {
        var assignments = await _staffAssignmentRepository.GetByRoleAsync(
            "StudentWork",
            request.WorkId,
            StaffRoleType.Reviewer,
            cancellationToken);

        var activeAssignment = assignments.FirstOrDefault(a => a.IsActive && !a.IsDeleted);
        if (activeAssignment == null)
        {
            return Result.Success<ReviewerDto?>(null);
        }

        var reviewer = await _reviewerRepository.GetByUserIdAsync(activeAssignment.UserId, cancellationToken);
        if (reviewer == null)
        {
            return Result.Success<ReviewerDto?>(null);
        }

        var dto = new ReviewerDto(
            reviewer.Id,
            reviewer.FullName,
            reviewer.Position,
            reviewer.AcademicDegree,
            reviewer.Organization,
            reviewer.Email,
            reviewer.Phone,
            reviewer.IsActive,
            reviewer.UserId);

        return Result.Success<ReviewerDto?>(dto);
    }
}

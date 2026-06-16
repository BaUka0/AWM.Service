using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Auth.Repositories;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.AssignReviewer;

public sealed class AssignReviewerCommandHandler : IRequestHandler<AssignReviewerCommand, Result>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IReviewerRepository _reviewerRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IRoleAccessRepository _roleAccessRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public AssignReviewerCommandHandler(
        IStudentWorkRepository studentWorkRepository,
        IReviewerRepository reviewerRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        IUserAccessRepository userAccessRepository,
        IRoleAccessRepository roleAccessRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _studentWorkRepository = studentWorkRepository;
        _reviewerRepository = reviewerRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
        _userAccessRepository = userAccessRepository;
        _roleAccessRepository = roleAccessRepository;
        _unitOfWork = unitOfWork;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result> Handle(AssignReviewerCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var work = await _studentWorkRepository.GetByIdAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure(new Error("StudentWorks.NotFound", $"Student work with ID {request.WorkId} not found."));
        }

        var reviewer = await _reviewerRepository.GetByIdAsync(request.ReviewerEntityId, cancellationToken);
        if (reviewer == null)
        {
            return Result.Failure(new Error("Reviewers.NotFound", $"Reviewer with ID {request.ReviewerEntityId} not found."));
        }

        if (!reviewer.UserId.HasValue)
        {
            return Result.Failure(new Error("Reviewers.NoSystemAccount",
                $"Reviewer '{reviewer.FullName}' does not have a linked system account and cannot be assigned."));
        }

        var reviewerUserId = reviewer.UserId.Value;

        var existingAssignments = await _staffAssignmentRepository.GetByRoleAsync(
            "StudentWork",
            request.WorkId,
            StaffRoleType.Reviewer,
            cancellationToken);

        var activeAssignments = existingAssignments.Where(a => a.IsActive && !a.IsDeleted).ToList();
        foreach (var assignment in activeAssignments)
        {
            assignment.Deactivate(currentUserId);
            await _staffAssignmentRepository.UpdateAsync(assignment, cancellationToken);

            var userAssignments = await _staffAssignmentRepository.GetByUserAsync(assignment.UserId, cancellationToken);
            var hasOtherActiveAssignments = userAssignments.Any(a =>
                a.IsActive &&
                !a.IsDeleted &&
                a.Id != assignment.Id &&
                a.RoleType == StaffRoleType.Reviewer);

            if (!hasOtherActiveAssignments)
            {
                var roleAccess = await _roleAccessRepository.GetByCodeAsync("REVIEWER", cancellationToken);
                if (roleAccess != null)
                {
                    var userAccessList = await _userAccessRepository.GetByUserIdAsync(assignment.UserId, cancellationToken);
                    var userAccess = userAccessList.FirstOrDefault(ua => ua.RoleAccessId == roleAccess.Id);
                    if (userAccess != null)
                    {
                        await _userAccessRepository.RemoveAsync(userAccess, cancellationToken);
                    }
                }
            }
        }

        var newAssignment = new StaffAssignment(
            reviewerUserId,
            StaffRoleType.Reviewer,
            "StudentWork",
            request.WorkId,
            currentUserId);

        await _staffAssignmentRepository.AddAsync(newAssignment, cancellationToken);

        var reviewerRole = await _roleAccessRepository.GetByCodeAsync("REVIEWER", cancellationToken);
        if (reviewerRole == null)
        {
            return Result.Failure(new Error("Roles.NotFound", "Role 'REVIEWER' not found in the system."));
        }

        if (!await _userAccessRepository.ExistsAsync(reviewerUserId, reviewerRole.Id, cancellationToken))
        {
            var userAccess = new UserAccess(reviewerUserId, reviewerRole.Id, currentUserId);
            await _userAccessRepository.AddAsync(userAccess, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

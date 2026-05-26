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
    private readonly IUserRepository _userRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly IUserAccessRepository _userAccessRepository;
    private readonly IRoleAccessRepository _roleAccessRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserProvider _currentUserProvider;

    public AssignReviewerCommandHandler(
        IStudentWorkRepository studentWorkRepository,
        IUserRepository userRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        IUserAccessRepository userAccessRepository,
        IRoleAccessRepository roleAccessRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserProvider currentUserProvider)
    {
        _studentWorkRepository = studentWorkRepository;
        _userRepository = userRepository;
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

        // Verify StudentWork exists
        var work = await _studentWorkRepository.GetByIdAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure(new Error("StudentWorks.NotFound", $"Student work with ID {request.WorkId} not found."));
        }

        // Verify Reviewer User exists
        var reviewerUser = await _userRepository.GetByIdAsync(request.ReviewerId, cancellationToken);
        if (reviewerUser == null)
        {
            return Result.Failure(new Error("Users.NotFound", $"Reviewer user with ID {request.ReviewerId} not found."));
        }

        // 1. Deactivate existing reviewer assignments for this work
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

            // Check if this deactivated user has any other active reviewer assignments in the system
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

        // 2. Create new StaffAssignment for the reviewer
        var newAssignment = new StaffAssignment(
            request.ReviewerId,
            StaffRoleType.Reviewer,
            "StudentWork",
            request.WorkId,
            currentUserId);

        await _staffAssignmentRepository.AddAsync(newAssignment, cancellationToken);

        // 3. Grant the REVIEWER role in UserAccesses to the user
        var reviewerRole = await _roleAccessRepository.GetByCodeAsync("REVIEWER", cancellationToken);
        if (reviewerRole == null)
        {
            return Result.Failure(new Error("Roles.NotFound", "Role 'REVIEWER' not found in the system."));
        }

        if (!await _userAccessRepository.ExistsAsync(request.ReviewerId, reviewerRole.Id, cancellationToken))
        {
            var userAccess = new UserAccess(request.ReviewerId, reviewerRole.Id, currentUserId);
            await _userAccessRepository.AddAsync(userAccess, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

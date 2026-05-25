using AWM.Service.Application.Features.Workflow.Attachments.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Attachments.Queries.GetWorkAttachments;

public sealed class GetWorkAttachmentsQueryHandler : IRequestHandler<GetWorkAttachmentsQuery, Result<IReadOnlyList<AttachmentDto>>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IAttachmentTypeRepository _attachmentTypeRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;

    public GetWorkAttachmentsQueryHandler(
        IStudentWorkRepository studentWorkRepository,
        IAttachmentTypeRepository attachmentTypeRepository,
        ITopicRepository topicRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        ICurrentUserProvider currentUserProvider)
    {
        _studentWorkRepository = studentWorkRepository;
        _attachmentTypeRepository = attachmentTypeRepository;
        _topicRepository = topicRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
        _currentUserProvider = currentUserProvider;
    }

    public async Task<Result<IReadOnlyList<AttachmentDto>>> Handle(GetWorkAttachmentsQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<AttachmentDto>>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure<IReadOnlyList<AttachmentDto>>(new Error("StudentWorks.NotFound", $"Student work with ID {request.WorkId} not found."));
        }

        // Verify read permissions:
        // - Participant
        // - Supervisor
        // - Assigned expert
        // - Department staff assignment
        var isParticipant = work.Participants.Any(p => p.StudentId == currentUserId);
        var isSupervisor = false;
        if (work.TopicId.HasValue)
        {
            var topic = await _topicRepository.GetByIdAsync(work.TopicId.Value, cancellationToken);
            isSupervisor = topic != null && topic.CreatedBy == currentUserId;
        }

        var isAssignedExpert = work.QualityChecks.Any(c => c.AssignedExpertId == currentUserId);
        
        var isStaffInDepartment = false;
        if (!isParticipant && !isSupervisor && !isAssignedExpert)
        {
            var userAssignments = await _staffAssignmentRepository.GetByUserAsync(currentUserId, cancellationToken);
            isStaffInDepartment = userAssignments.Any(a => 
                a.IsActive && !a.IsDeleted &&
                a.TargetEntityType == "OrgUnit" && 
                a.TargetEntityId == work.OrgUnitId &&
                (a.RoleType == StaffRoleType.CommissionMember ||
                 a.RoleType == StaffRoleType.QualityExpert ||
                 a.RoleType == StaffRoleType.CommissionChairman ||
                 a.RoleType == StaffRoleType.CommissionSecretary ||
                 a.RoleType == StaffRoleType.Supervisor));
        }

        if (!isParticipant && !isSupervisor && !isAssignedExpert && !isStaffInDepartment)
        {
            return Result.Failure<IReadOnlyList<AttachmentDto>>(new Error("Attachments.Forbidden", "You do not have permission to view attachments for this work."));
        }

        var attachmentTypes = await _attachmentTypeRepository.GetAllAsync(cancellationToken);
        var typeDict = attachmentTypes.ToDictionary(t => t.Id, t => t.Title);

        var dtos = work.Attachments.Select(a => new AttachmentDto(
            a.Id,
            a.WorkId,
            a.StateId,
            a.AttachmentTypeId,
            typeDict.TryGetValue(a.AttachmentTypeId, out var typeName) ? typeName : "Unknown",
            a.FileName,
            a.FileSizeBytes,
            a.ContentType,
            a.FileHash,
            a.CreatedAt,
            a.CreatedBy
        )).ToList();

        return Result.Success<IReadOnlyList<AttachmentDto>>(dtos);
    }
}

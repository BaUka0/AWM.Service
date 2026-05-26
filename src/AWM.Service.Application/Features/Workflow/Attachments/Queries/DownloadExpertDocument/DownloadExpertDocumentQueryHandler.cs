using AWM.Service.Application.Features.Workflow.Attachments.DTOs;
using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Service;
using KDS.Primitives.FluentResult;
using MediatR;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Attachments.Queries.DownloadExpertDocument;

public sealed class DownloadExpertDocumentQueryHandler : IRequestHandler<DownloadExpertDocumentQuery, Result<FileDownloadDto>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IAttachmentService _attachmentService;

    public DownloadExpertDocumentQueryHandler(
        IStudentWorkRepository studentWorkRepository,
        ITopicRepository topicRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        ICurrentUserProvider currentUserProvider,
        IAttachmentService attachmentService)
    {
        _studentWorkRepository = studentWorkRepository;
        _topicRepository = topicRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
        _currentUserProvider = currentUserProvider;
        _attachmentService = attachmentService;
    }

    public async Task<Result<FileDownloadDto>> Handle(DownloadExpertDocumentQuery request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<FileDownloadDto>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure<FileDownloadDto>(new Error("StudentWorks.NotFound", $"Student work with ID {request.WorkId} not found."));
        }

        var check = work.QualityChecks.FirstOrDefault(c => c.Id == request.QualityCheckId);
        if (check == null)
        {
            return Result.Failure<FileDownloadDto>(new Error("QualityChecks.NotFound", $"Quality check with ID {request.QualityCheckId} not found on this work."));
        }

        if (!check.AttachmentId.HasValue)
        {
            return Result.Failure<FileDownloadDto>(new Error("QualityChecks.NoDocument", "There is no document linked to this quality check."));
        }

        var attachment = work.Attachments.FirstOrDefault(a => a.Id == check.AttachmentId.Value);
        if (attachment == null)
        {
            return Result.Failure<FileDownloadDto>(new Error("Attachments.NotFound", $"Linked attachment with ID {check.AttachmentId.Value} not found on this work."));
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
                 a.RoleType == StaffRoleType.CommissionChairman ||
                 a.RoleType == StaffRoleType.CommissionSecretary ||
                 a.RoleType == StaffRoleType.Supervisor ||
                 (a.RoleType == StaffRoleType.QualityExpert && HasExpertCheckTypeAccess(a.MetadataJson, check.CheckTypeId))));
        }

        if (!isParticipant && !isSupervisor && !isAssignedExpert && !isStaffInDepartment)
        {
            return Result.Failure<FileDownloadDto>(new Error("Attachments.Forbidden", "You do not have permission to download this expert document."));
        }

        var fileStream = await _attachmentService.GetAsync(attachment.FileStoragePath, cancellationToken);
        var downloadDto = new FileDownloadDto(fileStream, attachment.FileName, attachment.ContentType);

        return Result.Success(downloadDto);
    }

    private static bool HasExpertCheckTypeAccess(string? metadataJson, int checkTypeId)
    {
        if (string.IsNullOrEmpty(metadataJson)) return false;
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(metadataJson);
            if (doc.RootElement.TryGetProperty("CheckTypeId", out var prop))
            {
                if (prop.ValueKind == System.Text.Json.JsonValueKind.Number)
                {
                    return prop.GetInt32() == checkTypeId;
                }
            }
        }
        catch { }
        return false;
    }
}

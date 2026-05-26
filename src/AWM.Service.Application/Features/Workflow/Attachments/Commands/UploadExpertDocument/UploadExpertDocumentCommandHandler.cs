using AWM.Service.Domain.Common;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.CommonDomain.Enums;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Service;
using KDS.Primitives.FluentResult;
using MediatR;
using Microsoft.Extensions.Options;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Attachments.Commands.UploadExpertDocument;

public sealed class UploadExpertDocumentCommandHandler : IRequestHandler<UploadExpertDocumentCommand, Result<long>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly IAttachmentTypeRepository _attachmentTypeRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IAttachmentService _attachmentService;
    private readonly StorageSettings _storageSettings;
    private readonly IUnitOfWork _unitOfWork;

    public UploadExpertDocumentCommandHandler(
        IStudentWorkRepository studentWorkRepository,
        IAttachmentTypeRepository attachmentTypeRepository,
        IStaffAssignmentRepository staffAssignmentRepository,
        ICurrentUserProvider currentUserProvider,
        IAttachmentService attachmentService,
        IOptions<StorageSettings> storageSettingsOptions,
        IUnitOfWork unitOfWork)
    {
        _studentWorkRepository = studentWorkRepository;
        _attachmentTypeRepository = attachmentTypeRepository;
        _staffAssignmentRepository = staffAssignmentRepository;
        _currentUserProvider = currentUserProvider;
        _attachmentService = attachmentService;
        _storageSettings = storageSettingsOptions.Value;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<long>> Handle(UploadExpertDocumentCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure<long>(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure<long>(new Error("StudentWorks.NotFound", $"Student work with ID {request.WorkId} not found."));
        }

        var check = work.QualityChecks.FirstOrDefault(c => c.Id == request.QualityCheckId);
        if (check == null)
        {
            return Result.Failure<long>(new Error("QualityChecks.NotFound", $"Quality check with ID {request.QualityCheckId} not found on this work."));
        }

        // Check rights: assigned expert or department expert/commission member
        var isAssignedExpert = check.AssignedExpertId.HasValue && check.AssignedExpertId.Value == currentUserId;
        var isExpertInDepartment = false;

        if (!isAssignedExpert)
        {
            var userAssignments = await _staffAssignmentRepository.GetByUserAsync(currentUserId, cancellationToken);
            isExpertInDepartment = userAssignments.Any(a => 
                a.IsActive && !a.IsDeleted &&
                a.TargetEntityType == "OrgUnit" && 
                a.TargetEntityId == work.OrgUnitId &&
                (a.RoleType == StaffRoleType.CommissionMember ||
                 a.RoleType == StaffRoleType.CommissionChairman ||
                 a.RoleType == StaffRoleType.CommissionSecretary ||
                 (a.RoleType == StaffRoleType.QualityExpert && HasExpertCheckTypeAccess(a.MetadataJson, check.CheckTypeId))));
        }

        if (!isAssignedExpert && !isExpertInDepartment)
        {
            return Result.Failure<long>(new Error("QualityChecks.Forbidden", "You do not have permission to upload expert documents for this check."));
        }

        // Verify attachment type
        var attachmentType = await _attachmentTypeRepository.GetByIdAsync(request.AttachmentTypeId, cancellationToken);
        if (attachmentType == null)
        {
            return Result.Failure<long>(new Error("AttachmentTypes.NotFound", $"Attachment type with ID {request.AttachmentTypeId} not found."));
        }

        // Validate size (experts use MaxReviewSizeMb or MaxAttachmentSizeMb)
        long maxSizeBytes = _storageSettings.MaxReviewSizeMb * 1024L * 1024L;
        if (request.FileSizeBytes > maxSizeBytes)
        {
            return Result.Failure<long>(new Error("Attachments.FileTooLarge", $"File size exceeds limit of {_storageSettings.MaxReviewSizeMb} MB."));
        }

        // Compute file hash
        var hash = await _attachmentService.ComputeHashAsync(request.FileStream, cancellationToken);

        // Reset stream position (just in case)
        if (request.FileStream.CanSeek)
        {
            request.FileStream.Position = 0;
        }

        // Save file physically
        var storagePath = await _attachmentService.SaveAsync(request.FileName, request.FileStream, request.ContentType, cancellationToken);

        // Add attachment in Domain
        var attachment = work.AddAttachment(
            request.AttachmentTypeId,
            request.FileName,
            storagePath,
            hash,
            currentUserId,
            request.FileSizeBytes,
            request.ContentType
        );

        // Save changes to generate attachment ID
        await _studentWorkRepository.UpdateAsync(work, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Link attachment to the quality check
        work.UpdateCheckAttachment(request.QualityCheckId, attachment.Id, currentUserId);

        await _studentWorkRepository.UpdateAsync(work, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(attachment.Id);
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

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

namespace AWM.Service.Application.Features.Workflow.Attachments.Queries.DownloadAttachment;

public sealed class DownloadAttachmentQueryHandler : IRequestHandler<DownloadAttachmentQuery, Result<FileDownloadDto>>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly IStaffAssignmentRepository _staffAssignmentRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IAttachmentService _attachmentService;

    public DownloadAttachmentQueryHandler(
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

    public async Task<Result<FileDownloadDto>> Handle(DownloadAttachmentQuery request, CancellationToken cancellationToken)
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

        var attachment = work.Attachments.FirstOrDefault(a => a.Id == request.AttachmentId);
        if (attachment == null)
        {
            return Result.Failure<FileDownloadDto>(new Error("Attachments.NotFound", $"Attachment with ID {request.AttachmentId} not found on this work."));
        }

        var isParticipant = work.Participants.Any(p => p.StudentId == currentUserId);
        var isSupervisor = false;
        if (work.TopicId.HasValue)
        {
            var topic = await _topicRepository.GetByIdAsync(work.TopicId.Value, cancellationToken);
            isSupervisor = topic != null && topic.CreatedBy == currentUserId;
        }

        var isAssignedExpert = work.QualityChecks.Any(c => c.AssignedExpertId == currentUserId);

        var isStaff = false;
        if (!isParticipant && !isSupervisor && !isAssignedExpert)
        {
            var userAssignments = await _staffAssignmentRepository.GetByUserAsync(currentUserId, cancellationToken);
            isStaff = userAssignments.Any(a => a.IsActive && !a.IsDeleted);
        }

        if (!isParticipant && !isSupervisor && !isAssignedExpert && !isStaff)
        {
            return Result.Failure<FileDownloadDto>(new Error("Attachments.Forbidden", "You do not have permission to download this attachment."));
        }

        var fileStream = await _attachmentService.GetAsync(attachment.FileStoragePath, cancellationToken);
        var downloadDto = new FileDownloadDto(fileStream, attachment.FileName, attachment.ContentType);

        return Result.Success(downloadDto);
    }
}

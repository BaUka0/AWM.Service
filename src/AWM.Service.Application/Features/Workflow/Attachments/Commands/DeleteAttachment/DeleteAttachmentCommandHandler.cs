using AWM.Service.Domain.Common;
using AWM.Service.Domain.Repositories;
using AWM.Service.Domain.Thesis.Service;
using KDS.Primitives.FluentResult;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AWM.Service.Application.Features.Workflow.Attachments.Commands.DeleteAttachment;

public sealed class DeleteAttachmentCommandHandler : IRequestHandler<DeleteAttachmentCommand, Result>
{
    private readonly IStudentWorkRepository _studentWorkRepository;
    private readonly ITopicRepository _topicRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IAttachmentService _attachmentService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAttachmentCommandHandler(
        IStudentWorkRepository studentWorkRepository,
        ITopicRepository topicRepository,
        ICurrentUserProvider currentUserProvider,
        IAttachmentService attachmentService,
        IUnitOfWork unitOfWork)
    {
        _studentWorkRepository = studentWorkRepository;
        _topicRepository = topicRepository;
        _currentUserProvider = currentUserProvider;
        _attachmentService = attachmentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteAttachmentCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserProvider.UserId.HasValue)
        {
            return Result.Failure(new Error("Auth.Unauthorized", "User is not authenticated."));
        }

        var currentUserId = _currentUserProvider.UserId.Value;

        var work = await _studentWorkRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work == null)
        {
            return Result.Failure(new Error("StudentWorks.NotFound", $"Student work with ID {request.WorkId} not found."));
        }

        var attachment = work.Attachments.FirstOrDefault(a => a.Id == request.AttachmentId);
        if (attachment == null)
        {
            return Result.Failure(new Error("Attachments.NotFound", $"Attachment with ID {request.AttachmentId} not found on this work."));
        }

        // Check rights: participant or supervisor
        var isParticipant = work.Participants.Any(p => p.StudentId == currentUserId);
        var isSupervisor = false;
        if (work.TopicId.HasValue)
        {
            var topic = await _topicRepository.GetByIdAsync(work.TopicId.Value, cancellationToken);
            isSupervisor = topic != null && topic.CreatedBy == currentUserId;
        }

        if (!isParticipant && !isSupervisor)
        {
            return Result.Failure(new Error("Attachments.Forbidden", "You do not have permission to delete this attachment."));
        }

        // Physically delete file
        await _attachmentService.DeleteAsync(attachment.FileStoragePath, cancellationToken);

        // Remove from domain collection
        work.RemoveAttachment(request.AttachmentId, currentUserId);

        await _studentWorkRepository.UpdateAsync(work, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

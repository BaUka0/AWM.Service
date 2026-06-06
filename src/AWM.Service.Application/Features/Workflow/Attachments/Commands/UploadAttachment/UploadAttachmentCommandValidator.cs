using AWM.Service.Domain.Common;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace AWM.Service.Application.Features.Workflow.Attachments.Commands.UploadAttachment;

public sealed class UploadAttachmentCommandValidator : AbstractValidator<UploadAttachmentCommand>
{
    public UploadAttachmentCommandValidator(IOptions<StorageSettings> storageSettings)
    {
        RuleFor(x => x.WorkId).GreaterThan(0);
        RuleFor(x => x.AttachmentTypeId).GreaterThan(0);
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(260);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(100);
        
        long maxSizeBytes = storageSettings.Value.MaxAttachmentSizeMb * 1024L * 1024L;
        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0)
            .LessThanOrEqualTo(maxSizeBytes)
            .WithMessage($"File size exceeds limit of {storageSettings.Value.MaxAttachmentSizeMb} MB.");
            
        RuleFor(x => x.FileStream).NotNull();
    }
}

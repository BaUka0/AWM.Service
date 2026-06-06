using AWM.Service.Domain.Common;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace AWM.Service.Application.Features.Workflow.Attachments.Commands.UploadExpertDocument;

public sealed class UploadExpertDocumentCommandValidator : AbstractValidator<UploadExpertDocumentCommand>
{
    public UploadExpertDocumentCommandValidator(IOptions<StorageSettings> storageSettings)
    {
        RuleFor(x => x.WorkId).GreaterThan(0);
        RuleFor(x => x.QualityCheckId).GreaterThan(0);
        RuleFor(x => x.AttachmentTypeId).GreaterThan(0);
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(260);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(100);
        
        long maxSizeBytes = storageSettings.Value.MaxReviewSizeMb * 1024L * 1024L;
        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0)
            .LessThanOrEqualTo(maxSizeBytes)
            .WithMessage($"File size exceeds limit of {storageSettings.Value.MaxReviewSizeMb} MB.");
            
        RuleFor(x => x.FileStream).NotNull();
    }
}

using AWM.Service.Domain.Common;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace AWM.Service.Application.Features.Workflow.Reviews.Commands.UploadRecension;

public sealed class UploadRecensionCommandValidator : AbstractValidator<UploadRecensionCommand>
{
    public UploadRecensionCommandValidator(IOptions<StorageSettings> storageSettings)
    {
        RuleFor(x => x.WorkId).GreaterThan(0);
        RuleFor(x => x.ReviewerUserId).GreaterThan(0).When(x => x.ReviewerUserId.HasValue);
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.ContentType).NotEmpty();
        
        long maxSizeBytes = storageSettings.Value.MaxAttachmentSizeMb * 1024L * 1024L;
        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0)
            .LessThanOrEqualTo(maxSizeBytes)
            .WithMessage($"File size exceeds limit of {storageSettings.Value.MaxAttachmentSizeMb} MB.");
            
        RuleFor(x => x.FileStream).NotNull();
    }
}

using AWM.Service.Domain.Common;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.SubmitSupervisorReview;

public sealed class SubmitSupervisorReviewCommandValidator : AbstractValidator<SubmitSupervisorReviewCommand>
{
    public SubmitSupervisorReviewCommandValidator(IOptions<StorageSettings> storageSettings)
    {
        RuleFor(x => x.WorkId).GreaterThan(0);
        RuleFor(x => x.FileName).NotEmpty().WithMessage("File name is required.");

        long maxSizeBytes = storageSettings.Value.MaxAttachmentSizeMb * 1024L * 1024L;
        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0)
            .LessThanOrEqualTo(maxSizeBytes)
            .WithMessage($"File size exceeds limit of {storageSettings.Value.MaxAttachmentSizeMb} MB.");

        RuleFor(x => x.FileStream).NotNull().WithMessage("File content is required.");
    }
}

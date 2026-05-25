using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Attachments.Commands.UploadExpertDocument;

public sealed class UploadExpertDocumentCommandValidator : AbstractValidator<UploadExpertDocumentCommand>
{
    public UploadExpertDocumentCommandValidator()
    {
        RuleFor(x => x.WorkId).GreaterThan(0);
        RuleFor(x => x.QualityCheckId).GreaterThan(0);
        RuleFor(x => x.AttachmentTypeId).GreaterThan(0);
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(260);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.FileSizeBytes).GreaterThan(0);
        RuleFor(x => x.FileStream).NotNull();
    }
}

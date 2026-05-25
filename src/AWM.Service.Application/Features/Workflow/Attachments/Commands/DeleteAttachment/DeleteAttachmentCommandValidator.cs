using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Attachments.Commands.DeleteAttachment;

public sealed class DeleteAttachmentCommandValidator : AbstractValidator<DeleteAttachmentCommand>
{
    public DeleteAttachmentCommandValidator()
    {
        RuleFor(x => x.WorkId).GreaterThan(0);
        RuleFor(x => x.AttachmentId).GreaterThan(0);
    }
}

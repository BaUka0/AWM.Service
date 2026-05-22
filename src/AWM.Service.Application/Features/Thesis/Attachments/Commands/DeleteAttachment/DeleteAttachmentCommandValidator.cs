using FluentValidation;

namespace AWM.Service.Application.Features.Thesis.Attachments.Commands.DeleteAttachment;

/// <summary>
/// Validator for DeleteAttachmentCommand.
/// </summary>
public class DeleteAttachmentCommandValidator : AbstractValidator<DeleteAttachmentCommand>
{
    public DeleteAttachmentCommandValidator()
    {
        RuleFor(x => x.AttachmentId)
            .GreaterThan(0).WithMessage("AttachmentId должен быть больше 0.");

        RuleFor(x => x.WorkId)
            .GreaterThan(0).WithMessage("WorkId должен быть больше 0.");
    }
}

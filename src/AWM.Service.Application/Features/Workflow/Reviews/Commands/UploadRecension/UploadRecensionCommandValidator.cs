using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Reviews.Commands.UploadRecension;

public sealed class UploadRecensionCommandValidator : AbstractValidator<UploadRecensionCommand>
{
    public UploadRecensionCommandValidator()
    {
        RuleFor(x => x.WorkId).GreaterThan(0);
        RuleFor(x => x.ReviewerUserId).GreaterThan(0);
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.ContentType).NotEmpty();
        RuleFor(x => x.FileSizeBytes).GreaterThan(0);
        RuleFor(x => x.FileStream).NotNull();
    }
}

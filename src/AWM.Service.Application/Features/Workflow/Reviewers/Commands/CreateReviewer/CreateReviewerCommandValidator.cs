using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Reviewers.Commands.CreateReviewer;

public sealed class CreateReviewerCommandValidator : AbstractValidator<CreateReviewerCommand>
{
    public CreateReviewerCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Position).MaximumLength(255);
        RuleFor(x => x.AcademicDegree).MaximumLength(100);
        RuleFor(x => x.Organization).MaximumLength(255);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email)).MaximumLength(255);
        RuleFor(x => x.Phone).MaximumLength(50);
    }
}

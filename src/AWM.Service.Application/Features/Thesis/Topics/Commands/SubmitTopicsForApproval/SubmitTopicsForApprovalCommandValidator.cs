using FluentValidation;

namespace AWM.Service.Application.Features.Thesis.Topics.Commands.SubmitTopicsForApproval;

/// <summary>
/// Validator for SubmitTopicsForApprovalCommand.
/// </summary>
public class SubmitTopicsForApprovalCommandValidator : AbstractValidator<SubmitTopicsForApprovalCommand>
{
    public SubmitTopicsForApprovalCommandValidator()
    {
        RuleFor(x => x.TopicIds)
            .NotEmpty().WithMessage("TopicIds не может быть пустым.");

        RuleForEach(x => x.TopicIds)
            .GreaterThan(0).WithMessage("TopicId должен быть больше 0.");
    }
}

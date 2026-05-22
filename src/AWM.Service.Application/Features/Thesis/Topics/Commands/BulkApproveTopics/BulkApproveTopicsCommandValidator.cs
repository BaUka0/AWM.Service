using FluentValidation;

namespace AWM.Service.Application.Features.Thesis.Topics.Commands.BulkApproveTopics;

/// <summary>
/// Validator for BulkApproveTopicsCommand.
/// </summary>
public class BulkApproveTopicsCommandValidator : AbstractValidator<BulkApproveTopicsCommand>
{
    public BulkApproveTopicsCommandValidator()
    {
        RuleFor(x => x.TopicIds)
            .NotEmpty().WithMessage("TopicIds не может быть пустым.");

        RuleForEach(x => x.TopicIds)
            .GreaterThan(0).WithMessage("TopicId должен быть больше 0.");
    }
}

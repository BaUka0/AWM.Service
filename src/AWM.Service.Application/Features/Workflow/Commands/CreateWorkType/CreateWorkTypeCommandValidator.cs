using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Commands.CreateWorkType;

/// <summary>
/// Validator for CreateWorkTypeCommand.
/// </summary>
public class CreateWorkTypeCommandValidator : AbstractValidator<CreateWorkTypeCommand>
{
    public CreateWorkTypeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name обязателен.")
            .MaximumLength(200).WithMessage("Name не должен превышать 200 символов.");
    }
}

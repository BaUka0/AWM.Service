using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Commands.UpdateWorkType;

/// <summary>
/// Validator for UpdateWorkTypeCommand.
/// </summary>
public class UpdateWorkTypeCommandValidator : AbstractValidator<UpdateWorkTypeCommand>
{
    public UpdateWorkTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id должен быть больше 0.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name обязателен.")
            .MaximumLength(200).WithMessage("Name не должен превышать 200 символов.");
    }
}

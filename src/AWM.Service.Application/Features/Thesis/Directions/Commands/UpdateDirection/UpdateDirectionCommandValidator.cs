namespace AWM.Service.Application.Features.Thesis.Directions.Commands.UpdateDirection;

using FluentValidation;

/// <summary>
/// Validator for UpdateDirectionCommand.
/// </summary>
public sealed class UpdateDirectionCommandValidator
    : AbstractValidator<UpdateDirectionCommand>
{
    public UpdateDirectionCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Direction ID must be greater than 0.");

        RuleFor(x => x.TitleRu)
            .NotEmpty()
            .WithMessage("Russian title is required.")
            .MaximumLength(500)
            .WithMessage("Russian title cannot exceed 500 characters.");

        RuleFor(x => x.TitleKz)
            .MaximumLength(500)
            .WithMessage("Kazakh title cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.TitleKz));

        RuleFor(x => x.TitleEn)
            .MaximumLength(500)
            .WithMessage("English title cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.TitleEn));

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

    }
}
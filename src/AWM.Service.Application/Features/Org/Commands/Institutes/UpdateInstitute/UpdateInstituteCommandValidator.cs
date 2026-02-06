namespace AWM.Service.Application.Features.Org.Commands.Institutes.UpdateInstitute;

using FluentValidation;

/// <summary>
/// Validator for UpdateInstituteCommand.
/// </summary>
public sealed class UpdateInstituteCommandValidator : AbstractValidator<UpdateInstituteCommand>
{
    public UpdateInstituteCommandValidator()
    {
        RuleFor(x => x.InstituteId)
            .GreaterThan(0)
            .WithMessage("Institute ID must be greater than 0.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Institute name is required.")
            .MaximumLength(200)
            .WithMessage("Institute name must not exceed 200 characters.");
    }
}

namespace AWM.Service.Application.Features.Org.Commands.Institutes.CreateInstitute;

using FluentValidation;

/// <summary>
/// Validator for CreateInstituteCommand.
/// </summary>
public sealed class CreateInstituteCommandValidator : AbstractValidator<CreateInstituteCommand>
{
    public CreateInstituteCommandValidator()
    {
        RuleFor(x => x.UniversityId)
            .GreaterThan(0)
            .WithMessage("University ID must be greater than 0.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Institute name is required.")
            .MaximumLength(200)
            .WithMessage("Institute name must not exceed 200 characters.")
            .Matches(@"^[a-zA-Z0-9\s\-\.,']+$")
            .WithMessage("Institute name contains invalid characters.");
    }
}

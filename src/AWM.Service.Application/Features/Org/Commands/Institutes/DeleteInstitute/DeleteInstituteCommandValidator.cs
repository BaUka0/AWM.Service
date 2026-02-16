namespace AWM.Service.Application.Features.Org.Commands.Institutes.DeleteInstitute;

using FluentValidation;

/// <summary>
/// Validator for DeleteInstituteCommand.
/// </summary>
public sealed class DeleteInstituteCommandValidator : AbstractValidator<DeleteInstituteCommand>
{
    public DeleteInstituteCommandValidator()
    {
        RuleFor(x => x.InstituteId)
            .GreaterThan(0)
            .WithMessage("Institute ID must be greater than 0.");

        RuleFor(x => x.DeletedBy)
            .GreaterThan(0)
            .WithMessage("DeletedBy must be a valid user ID.");
    }
}

namespace AWM.Service.Application.Features.Thesis.Works.Commands.AddParticipant;

using FluentValidation;

/// <summary>
/// Validator for AddParticipantCommand.
/// </summary>
public sealed class AddParticipantCommandValidator : AbstractValidator<AddParticipantCommand>
{
    public AddParticipantCommandValidator()
    {
        RuleFor(x => x.WorkId)
            .GreaterThan(0)
            .WithMessage("WorkId must be a positive number.");

        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("StudentId must be a positive number.");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Role must be a valid participant role.");
    }
}

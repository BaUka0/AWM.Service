using FluentValidation;

namespace AWM.Service.Application.Features.Thesis.Works.Commands.RemoveParticipant;

/// <summary>
/// Validator for RemoveParticipantCommand.
/// </summary>
public class RemoveParticipantCommandValidator : AbstractValidator<RemoveParticipantCommand>
{
    public RemoveParticipantCommandValidator()
    {
        RuleFor(x => x.WorkId)
            .GreaterThan(0).WithMessage("WorkId должен быть больше 0.");

        RuleFor(x => x.StudentId)
            .GreaterThan(0).WithMessage("StudentId должен быть больше 0.");
    }
}

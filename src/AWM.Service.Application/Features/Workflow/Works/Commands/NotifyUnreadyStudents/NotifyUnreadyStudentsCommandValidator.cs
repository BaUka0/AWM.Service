using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.NotifyUnreadyStudents;

/// <summary>
/// Validator for NotifyUnreadyStudentsCommand.
/// </summary>
public sealed class NotifyUnreadyStudentsCommandValidator : AbstractValidator<NotifyUnreadyStudentsCommand>
{
    public NotifyUnreadyStudentsCommandValidator()
    {
        RuleFor(x => x.OrgUnitId)
            .GreaterThan(0)
            .WithMessage("OrgUnitId must be greater than 0.");

        RuleFor(x => x.SemesterId)
            .GreaterThan(0)
            .WithMessage("SemesterId must be greater than 0.");
    }
}

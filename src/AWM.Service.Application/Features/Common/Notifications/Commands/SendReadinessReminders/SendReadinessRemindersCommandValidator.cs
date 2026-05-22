using FluentValidation;

namespace AWM.Service.Application.Features.Common.Notifications.Commands.SendReadinessReminders;

/// <summary>
/// Validator for SendReadinessRemindersCommand.
/// </summary>
public class SendReadinessRemindersCommandValidator : AbstractValidator<SendReadinessRemindersCommand>
{
    public SendReadinessRemindersCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("DepartmentId должен быть больше 0.");

        RuleFor(x => x.AcademicYearId)
            .GreaterThan(0).WithMessage("AcademicYearId должен быть больше 0.");
    }
}

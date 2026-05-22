using FluentValidation;

namespace AWM.Service.Application.Features.Common.Notifications.Commands.MarkAsRead;

/// <summary>
/// Validator for MarkAsReadCommand.
/// </summary>
public class MarkAsReadCommandValidator : AbstractValidator<MarkAsReadCommand>
{
    public MarkAsReadCommandValidator()
    {
        RuleFor(x => x.NotificationId)
            .GreaterThan(0).WithMessage("NotificationId должен быть больше 0.");
    }
}

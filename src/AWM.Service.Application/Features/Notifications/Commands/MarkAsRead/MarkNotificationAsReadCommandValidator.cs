using FluentValidation;

namespace AWM.Service.Application.Features.Notifications.Commands.MarkAsRead;

public sealed class MarkNotificationAsReadCommandValidator : AbstractValidator<MarkNotificationAsReadCommand>
{
    public MarkNotificationAsReadCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

using FluentValidation;

namespace AWM.Service.Application.Features.Common.Notifications.Commands.MarkAllAsRead;

/// <summary>
/// Validator for MarkAllAsReadCommand.
/// </summary>
public class MarkAllAsReadCommandValidator : AbstractValidator<MarkAllAsReadCommand>
{
    public MarkAllAsReadCommandValidator()
    {
        // No properties to validate
    }
}

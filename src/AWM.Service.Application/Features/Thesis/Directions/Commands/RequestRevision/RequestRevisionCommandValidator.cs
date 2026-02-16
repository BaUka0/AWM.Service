namespace AWM.Service.Application.Features.Thesis.Directions.Commands.RequestRevision;

using FluentValidation;

/// <summary>
/// Validator for RequestRevisionCommand.
/// </summary>
public sealed class RequestRevisionCommandValidator 
    : AbstractValidator<RequestRevisionCommand>
{
    public RequestRevisionCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Direction ID must be greater than 0.");

        RuleFor(x => x.RequestedBy)
            .GreaterThan(0)
            .WithMessage("RequestedBy user ID must be greater than 0.");

        RuleFor(x => x.Comment)
            .NotEmpty()
            .WithMessage("Revision comment is required. Supervisor needs to know what needs to be fixed.")
            .MaximumLength(1000)
            .WithMessage("Revision comment cannot exceed 1000 characters.");
    }
}
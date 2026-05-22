using FluentValidation;

namespace AWM.Service.Application.Features.Thesis.Directions.Commands.ApproveDirection;

/// <summary>
/// Validator for ApproveDirectionCommand.
/// </summary>
public class ApproveDirectionCommandValidator : AbstractValidator<ApproveDirectionCommand>
{
    public ApproveDirectionCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id должен быть больше 0.");
    }
}

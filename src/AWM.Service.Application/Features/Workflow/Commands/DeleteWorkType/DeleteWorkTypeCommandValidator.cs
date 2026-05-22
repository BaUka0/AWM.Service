using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Commands.DeleteWorkType;

/// <summary>
/// Validator for DeleteWorkTypeCommand.
/// </summary>
public class DeleteWorkTypeCommandValidator : AbstractValidator<DeleteWorkTypeCommand>
{
    public DeleteWorkTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id должен быть больше 0.");
    }
}

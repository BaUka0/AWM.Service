using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.WorkTypes.Commands.DeleteWorkType;

public sealed class DeleteWorkTypeCommandValidator : AbstractValidator<DeleteWorkTypeCommand>
{
    public DeleteWorkTypeCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

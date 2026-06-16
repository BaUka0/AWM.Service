using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Checks.Commands.DeleteCheckConfiguration;

public sealed class DeleteCheckConfigurationCommandValidator : AbstractValidator<DeleteCheckConfigurationCommand>
{
    public DeleteCheckConfigurationCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

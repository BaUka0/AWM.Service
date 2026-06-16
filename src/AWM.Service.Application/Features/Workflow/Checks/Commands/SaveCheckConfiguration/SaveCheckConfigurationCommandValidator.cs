using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Checks.Commands.SaveCheckConfiguration;

public sealed class SaveCheckConfigurationCommandValidator : AbstractValidator<SaveCheckConfigurationCommand>
{
    public SaveCheckConfigurationCommandValidator()
    {
        RuleFor(x => x.OrgUnitId).GreaterThan(0);
        RuleFor(x => x.CheckTypeId).GreaterThan(0);
        RuleFor(x => x.MinimumPassValue)
            .InclusiveBetween(0, 100)
            .When(x => x.MinimumPassValue.HasValue);
    }
}

using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Stages.Commands.ResetStages;

public sealed class ResetStagesCommandValidator : AbstractValidator<ResetStagesCommand>
{
    public ResetStagesCommandValidator()
    {
        RuleFor(x => x.SemesterId)
            .GreaterThan(0).WithMessage("SemesterId must be greater than 0.");

        RuleFor(x => x.SpecialityId)
            .GreaterThan(0).WithMessage("SpecialityId must be greater than 0.");
    }
}

using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.AdmitToDefense;

public sealed class AdmitToDefenseCommandValidator : AbstractValidator<AdmitToDefenseCommand>
{
    public AdmitToDefenseCommandValidator()
    {
        RuleFor(x => x.WorkId).GreaterThan(0);
    }
}

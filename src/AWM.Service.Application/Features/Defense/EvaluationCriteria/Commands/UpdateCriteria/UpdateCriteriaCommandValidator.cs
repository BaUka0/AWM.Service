using FluentValidation;

namespace AWM.Service.Application.Features.Defense.EvaluationCriteria.Commands.UpdateCriteria;

public sealed class UpdateCriteriaCommandValidator : AbstractValidator<UpdateCriteriaCommand>
{
    public UpdateCriteriaCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.CriteriaName).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.MaxScore).GreaterThan(0);
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DefenseStageType).GreaterThan(0).When(x => x.DefenseStageType.HasValue);
    }
}

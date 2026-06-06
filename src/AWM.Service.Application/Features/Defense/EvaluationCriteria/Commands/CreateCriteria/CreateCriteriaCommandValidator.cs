using FluentValidation;

namespace AWM.Service.Application.Features.Defense.EvaluationCriteria.Commands.CreateCriteria;

public sealed class CreateCriteriaCommandValidator : AbstractValidator<CreateCriteriaCommand>
{
    public CreateCriteriaCommandValidator()
    {
        RuleFor(x => x.WorkTypeId).GreaterThan(0);
        RuleFor(x => x.CriteriaName).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.MaxScore).GreaterThan(0);
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0);
        RuleFor(x => x.OrgUnitId).GreaterThan(0).When(x => x.OrgUnitId.HasValue);
        RuleFor(x => x.SpecialityId).GreaterThan(0).When(x => x.SpecialityId.HasValue);
        RuleFor(x => x.DefenseStageType).GreaterThan(0).When(x => x.DefenseStageType.HasValue);
    }
}

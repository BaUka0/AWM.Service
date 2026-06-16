using FluentValidation;

namespace AWM.Service.Application.Features.Defense.EvaluationCriteria.Commands.DeleteCriteria;

public sealed class DeleteCriteriaCommandValidator : AbstractValidator<DeleteCriteriaCommand>
{
    public DeleteCriteriaCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}

using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.WorkTypes.Commands.UpdateWorkType;

public sealed class UpdateWorkTypeCommandValidator : AbstractValidator<UpdateWorkTypeCommand>
{
    public UpdateWorkTypeCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(500);
        RuleFor(x => x.SpecialityLevelId).GreaterThan(0).When(x => x.SpecialityLevelId.HasValue);
    }
}

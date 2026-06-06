using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.WorkTypes.Commands.CreateWorkType;

public sealed class CreateWorkTypeCommandValidator : AbstractValidator<CreateWorkTypeCommand>
{
    public CreateWorkTypeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(500);
        RuleFor(x => x.SpecialityLevelId).GreaterThan(0).When(x => x.SpecialityLevelId.HasValue);
    }
}

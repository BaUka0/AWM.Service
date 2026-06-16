using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.MarkAsGraduated;

public sealed class MarkAsGraduatedCommandValidator : AbstractValidator<MarkAsGraduatedCommand>
{
    public MarkAsGraduatedCommandValidator()
    {
        RuleFor(x => x.WorkIds)
            .NotEmpty()
            .WithMessage("At least one WorkId is required.");
    }
}

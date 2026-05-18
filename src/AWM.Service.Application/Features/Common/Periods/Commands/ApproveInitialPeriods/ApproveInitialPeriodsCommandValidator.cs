namespace AWM.Service.Application.Features.Common.Stages.Commands.ApproveInitialStages;

using FluentValidation;

public sealed class ApproveInitialStagesCommandValidator : AbstractValidator<ApproveInitialStagesCommand>
{
    public ApproveInitialStagesCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("Department ID must be specified.");

        RuleFor(x => x.SemesterId)
            .GreaterThan(0)
            .WithMessage("Semester ID must be specified.");

        RuleFor(x => x.Stages)
            .NotEmpty()
            .WithMessage("At least one stage must be provided.");

        RuleForEach(x => x.Stages).ChildRules(stage =>
        {
            stage.RuleFor(s => s.EndDate)
                .GreaterThan(s => s.StartDate)
                .WithMessage("End date must be after start date.");
        });
    }
}

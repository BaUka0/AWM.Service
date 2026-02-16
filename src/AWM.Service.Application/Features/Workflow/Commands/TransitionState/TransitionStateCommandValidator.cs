namespace AWM.Service.Application.Features.Workflow.Commands.TransitionState;

using FluentValidation;

public sealed class TransitionStateCommandValidator : AbstractValidator<TransitionStateCommand>
{
    private static readonly string[] SupportedEntityTypes = { "direction", "studentwork", "work" };

    public TransitionStateCommandValidator()
    {
        RuleFor(x => x.EntityType)
            .NotEmpty().WithMessage("Entity type is required.")
            .Must(t => SupportedEntityTypes.Contains(t.ToLowerInvariant()))
            .WithMessage("Entity type must be one of: Direction, StudentWork, Work.");

        RuleFor(x => x.EntityId)
            .GreaterThan(0).WithMessage("Entity ID must be greater than 0.");

        RuleFor(x => x.TargetStateId)
            .GreaterThan(0).WithMessage("Target State ID must be greater than 0.");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.")
            .When(x => x.Comment is not null);
    }
}

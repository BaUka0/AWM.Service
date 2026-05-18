namespace AWM.Service.Application.Features.Common.Stages.Commands.CreateStage;

using FluentValidation;

public sealed class CreateStageCommandValidator : AbstractValidator<CreateStageCommand>
{
    public CreateStageCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("Department ID must be greater than 0.");

        RuleFor(x => x.SemesterId)
            .GreaterThan(0).WithMessage("Semester ID must be greater than 0.");

        RuleFor(x => x.WorkflowStageId)
            .GreaterThan(0).WithMessage("Invalid workflow stage.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");
    }
}

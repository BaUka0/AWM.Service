namespace AWM.Service.Application.Features.Common.Periods.Commands.CreatePeriod;

using FluentValidation;

public sealed class CreatePeriodCommandValidator : AbstractValidator<CreatePeriodCommand>
{
    public CreatePeriodCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("Department ID must be greater than 0.");

        RuleFor(x => x.AcademicYearId)
            .GreaterThan(0).WithMessage("Academic Year ID must be greater than 0.");

        RuleFor(x => x.WorkflowStage)
            .IsInEnum().WithMessage("Invalid workflow stage.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");
    }
}

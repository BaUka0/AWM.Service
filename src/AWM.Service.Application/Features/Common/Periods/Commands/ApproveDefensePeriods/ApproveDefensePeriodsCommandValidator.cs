using FluentValidation;

namespace AWM.Service.Application.Features.Common.Stages.Commands.ApproveDefenseStages;

/// <summary>
/// Validator for ApproveDefenseStagesCommand.
/// </summary>
public class ApproveDefenseStagesCommandValidator : AbstractValidator<ApproveDefenseStagesCommand>
{
    public ApproveDefenseStagesCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("DepartmentId должен быть больше 0.");

        RuleFor(x => x.SemesterId)
            .GreaterThan(0).WithMessage("SemesterId должен быть больше 0.");

        RuleFor(x => x.Stages)
            .NotEmpty().WithMessage("Stages не может быть пустым.");
    }
}

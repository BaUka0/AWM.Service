using FluentValidation;

namespace AWM.Service.Application.Features.Defense.PreDefense.Commands.DistributeStudentsToCommissions;

/// <summary>
/// Validator for DistributeStudentsToCommissionsCommand.
/// </summary>
public class DistributeStudentsToCommissionsCommandValidator : AbstractValidator<DistributeStudentsToCommissionsCommand>
{
    public DistributeStudentsToCommissionsCommandValidator()
    {
        RuleFor(x => x.OrgUnitId)
            .GreaterThan(0).WithMessage("OrgUnitId должен быть больше 0.");

        RuleFor(x => x.SemesterId)
            .GreaterThan(0).WithMessage("SemesterId должен быть больше 0.");

        RuleFor(x => x.PreDefenseNumber)
            .InclusiveBetween(1, 3).WithMessage("PreDefenseNumber должен быть от 1 до 3.");
    }
}

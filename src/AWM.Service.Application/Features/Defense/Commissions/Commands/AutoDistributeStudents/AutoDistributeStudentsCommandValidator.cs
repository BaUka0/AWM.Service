using FluentValidation;

namespace AWM.Service.Application.Features.Defense.Commissions.Commands.AutoDistributeStudents;

public sealed class AutoDistributeStudentsCommandValidator : AbstractValidator<AutoDistributeStudentsCommand>
{
    public AutoDistributeStudentsCommandValidator()
    {
        RuleFor(x => x.OrgUnitId).GreaterThan(0);
        RuleFor(x => x.SemesterId).GreaterThan(0);
        RuleFor(x => x.CommissionTypeId).InclusiveBetween(1, 2);

        RuleFor(x => x.PreDefenseNumber)
            .InclusiveBetween(1, 3)
            .When(x => x.CommissionTypeId == 1);
    }
}

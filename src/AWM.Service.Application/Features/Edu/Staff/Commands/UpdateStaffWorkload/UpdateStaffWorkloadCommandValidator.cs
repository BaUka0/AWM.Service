namespace AWM.Service.Application.Features.Edu.Staff.Commands.UpdateStaffWorkload;

using FluentValidation;

public sealed class UpdateStaffWorkloadCommandValidator : AbstractValidator<UpdateStaffWorkloadCommand>
{
    public UpdateStaffWorkloadCommandValidator()
    {
        RuleFor(x => x.StaffId)
            .GreaterThan(0)
            .WithMessage("StaffId must be greater than zero.");

        RuleFor(x => x.MaxStudentsLoad)
            .GreaterThanOrEqualTo(0)
            .WithMessage("MaxStudentsLoad cannot be negative.");
    }
}

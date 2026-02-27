namespace AWM.Service.Application.Features.Edu.Staff.Commands.UpdateStaff;

using FluentValidation;

public sealed class UpdateStaffCommandValidator : AbstractValidator<UpdateStaffCommand>
{
    public UpdateStaffCommandValidator()
    {
        RuleFor(x => x.StaffId)
            .GreaterThan(0).WithMessage("Staff ID must be greater than 0.");

        RuleFor(x => x.Position)
            .MaximumLength(200).WithMessage("Position must not exceed 200 characters.")
            .When(x => x.Position is not null);

        RuleFor(x => x.AcademicDegree)
            .MaximumLength(200).WithMessage("Academic degree must not exceed 200 characters.")
            .When(x => x.AcademicDegree is not null);

        RuleFor(x => x.MaxStudentsLoad)
            .GreaterThan(0).WithMessage("Max students load must be greater than 0.")
            .LessThanOrEqualTo(50).WithMessage("Max students load cannot exceed 50.")
            .When(x => x.MaxStudentsLoad.HasValue);

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("Department ID must be greater than 0.")
            .When(x => x.DepartmentId.HasValue);
    }
}

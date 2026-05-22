using FluentValidation;

namespace AWM.Service.Application.Features.Thesis.Topics.Commands.CompleteTopicCoordination;

/// <summary>
/// Validator for CompleteTopicCoordinationCommand.
/// </summary>
public class CompleteTopicCoordinationCommandValidator : AbstractValidator<CompleteTopicCoordinationCommand>
{
    public CompleteTopicCoordinationCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("DepartmentId должен быть больше 0.");

        RuleFor(x => x.AcademicYearId)
            .GreaterThan(0).WithMessage("AcademicYearId должен быть больше 0.");
    }
}

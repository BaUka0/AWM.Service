namespace AWM.Service.Application.Features.Thesis.Topics.Commands.CreateTopic;

using FluentValidation;

/// <summary>
/// Validator for CreateTopicCommand.
/// </summary>
public sealed class CreateTopicCommandValidator : AbstractValidator<CreateTopicCommand>
{
    public CreateTopicCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0)
            .WithMessage("Department ID must be greater than 0.");

        RuleFor(x => x.SupervisorId)
            .GreaterThan(0)
            .WithMessage("Supervisor ID must be greater than 0.");

        RuleFor(x => x.AcademicYearId)
            .GreaterThan(0)
            .WithMessage("Academic year ID must be greater than 0.");

        RuleFor(x => x.WorkTypeId)
            .GreaterThan(0)
            .WithMessage("Work type ID must be greater than 0.");

        RuleFor(x => x.DirectionId)
            .GreaterThan(0)
            .WithMessage("Direction ID must be greater than 0.")
            .When(x => x.DirectionId.HasValue);

        RuleFor(x => x.TitleRu)
            .NotEmpty()
            .WithMessage("Russian title is required.")
            .MaximumLength(500)
            .WithMessage("Russian title must not exceed 500 characters.");

        RuleFor(x => x.TitleKz)
            .MaximumLength(500)
            .WithMessage("Kazakh title must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TitleKz));

        RuleFor(x => x.TitleEn)
            .MaximumLength(500)
            .WithMessage("English title must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.TitleEn));

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description must not exceed 2000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.MaxParticipants)
            .InclusiveBetween(1, 5)
            .WithMessage("Max participants must be between 1 and 5.");

        RuleFor(x => x.CreatedBy)
            .GreaterThan(0)
            .WithMessage("CreatedBy must be a valid user ID.");
    }
}
namespace AWM.Service.Application.Features.Common.Stages.Commands.UpdateStage;

using FluentValidation;

public sealed class UpdateStageCommandValidator : AbstractValidator<UpdateStageCommand>
{
    public UpdateStageCommandValidator()
    {
        RuleFor(x => x.StageId)
            .GreaterThan(0).WithMessage("Stage ID must be greater than 0.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}

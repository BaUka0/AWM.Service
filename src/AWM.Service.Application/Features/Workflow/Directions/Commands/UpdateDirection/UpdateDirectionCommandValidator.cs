using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Directions.Commands.UpdateDirection;

public sealed class UpdateDirectionCommandValidator : AbstractValidator<UpdateDirectionCommand>
{
    public UpdateDirectionCommandValidator()
    {
        RuleFor(v => v.DirectionId).GreaterThan(0);
        RuleFor(v => v.TitleRu).NotEmpty().MaximumLength(2000);
    }
}

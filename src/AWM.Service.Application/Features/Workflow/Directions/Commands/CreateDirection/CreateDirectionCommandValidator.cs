using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Directions.Commands.CreateDirection;

public sealed class CreateDirectionCommandValidator : AbstractValidator<CreateDirectionCommand>
{
    public CreateDirectionCommandValidator()
    {
        RuleFor(v => v.SemesterId).GreaterThan(0);
        RuleFor(v => v.WorkTypeId).GreaterThan(0);
        RuleFor(v => v.TitleRu).NotEmpty().MaximumLength(2000);
        RuleFor(v => v.OrgUnitId).GreaterThan(0).When(v => v.OrgUnitId.HasValue);
    }
}

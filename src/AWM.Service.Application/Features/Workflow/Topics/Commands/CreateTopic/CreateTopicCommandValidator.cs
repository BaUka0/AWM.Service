using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Topics.Commands.CreateTopic;

public sealed class CreateTopicCommandValidator : AbstractValidator<CreateTopicCommand>
{
    public CreateTopicCommandValidator()
    {
        RuleFor(x => x.SemesterId).GreaterThan(0);
        RuleFor(x => x.WorkTypeId).GreaterThan(0);
        RuleFor(x => x.TitleRu).NotEmpty().MaximumLength(500);
        RuleFor(x => x.MaxParticipants).InclusiveBetween(1, 3);

        // Optional field validations
        RuleFor(x => x.TitleKz).MaximumLength(500).When(x => x.TitleKz != null);
        RuleFor(x => x.TitleEn).MaximumLength(500).When(x => x.TitleEn != null);
        RuleFor(x => x.DescriptionRu).MaximumLength(4000).When(x => x.DescriptionRu != null);
        RuleFor(x => x.DescriptionKz).MaximumLength(4000).When(x => x.DescriptionKz != null);
        RuleFor(x => x.DescriptionEn).MaximumLength(4000).When(x => x.DescriptionEn != null);
        RuleFor(x => x.DirectionId).GreaterThan(0).When(x => x.DirectionId.HasValue);
        RuleFor(x => x.SpecialityId).GreaterThan(0).When(x => x.SpecialityId.HasValue);
        RuleFor(x => x.OrgUnitId).GreaterThan(0).When(x => x.OrgUnitId.HasValue);
    }
}

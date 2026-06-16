using FluentValidation;

namespace AWM.Service.Application.Features.Workflow.Works.Commands.SaveRepoUrl;

public sealed class SaveRepoUrlCommandValidator : AbstractValidator<SaveRepoUrlCommand>
{
    public SaveRepoUrlCommandValidator()
    {
        RuleFor(x => x.WorkId).GreaterThan(0);
        RuleFor(x => x.RepoUrl).NotEmpty().MaximumLength(2000);
    }
}

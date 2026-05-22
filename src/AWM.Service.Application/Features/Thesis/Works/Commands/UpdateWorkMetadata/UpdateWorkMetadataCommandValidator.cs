using FluentValidation;

namespace AWM.Service.Application.Features.Thesis.Works.Commands.UpdateWorkMetadata;

/// <summary>
/// Validator for UpdateWorkMetadataCommand.
/// </summary>
public class UpdateWorkMetadataCommandValidator : AbstractValidator<UpdateWorkMetadataCommand>
{
    public UpdateWorkMetadataCommandValidator()
    {
        RuleFor(x => x.WorkId)
            .GreaterThan(0).WithMessage("WorkId должен быть больше 0.");
    }
}
